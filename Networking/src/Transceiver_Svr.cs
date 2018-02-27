using System;
using System.Collections.Concurrent; // CocurrentQueue
using System.Collections.Generic; // List
using System.IO; // MemoryStream
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter
using System.Threading;

namespace COMP4981_NetworkingTest
{
    /// <summary>
    /// Server driver to send and receive game updates.
    /// 
    /// Author: Jeremy L
    /// </summary>
    public class Transceiver_Svr
    {
        private const int BUFF_SIZE = 1200; // buffer size
        private const int MAX_CLIENTS = 30; // max number of client conns

        // Sender and Receiver
        private Thread thrSender;
        private Thread thrDatagramRcvr;
        private Thread thrRxQueueReader;
        private bool runSender;
        private bool runReceiver;
        // data structures
        private ConcurrentQueue<byte[]> updateQueueToSend;
        private ConcurrentQueue<byte[]> rcvdDatagramQueue; // rx datagram queue
        private List<Connection> connPool; // connection pool

        /// <summary>
        /// Constructor for a Transceiver object.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public Transceiver_Svr()
        {
            this.connPool = new List<Connection>();
            this.runSender = false;
            this.runReceiver = false;
            this.updateQueueToSend = new ConcurrentQueue<byte[]>();
            this.rcvdDatagramQueue = new ConcurrentQueue<byte[]>();
        }

        ~Transceiver_Svr()
        {
            StopSender();
            StopReceiver();
        }

        #region // Transceiver -----------------------------------------
        /// <summary>
        /// 
        /// Author: Willson Hu
        /// </summary>
        public void AddConnection()
        {
            connPool.Add(new Connection());
        }

        /// <summary>
        /// Test version.
        /// 
        /// TODO: delete after testing data flow.
        /// 
        /// Author: Willson Hu, Jeremy Lee
        /// </summary>
        public void AddConnection(int i)
        {
            connPool.Add(new Connection(i));
        }

        /// <summary>
        /// 
        /// Author: Willson Hu
        /// </summary>
        public bool RemoveConnection()
        {
            return false;
        }

        /// <summary>
        /// 
        /// Author: Willson Hu
        /// </summary>
        public int GetNumConnections()
        {
            return connPool.Count;
        }
        #endregion // Transceiver --------------------------------------

        #region // Sender ----------------------------------------------
        /// <summary>
        /// Starts Sender on a separate thread. Use this function to
        /// start sending queued game updates to all clients.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public void StartSender()
        {
            this.runSender = true;
            this.thrSender = new Thread(sendUpdateToClients);
            this.thrSender.Start();
        }

        /// <summary>
        /// Stops sender.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public void StopSender()
        {
            this.runSender = false;
            Thread.Sleep(1000); // sleep for cleaning up
            this.thrSender = null;
        }

        /// <summary>
        /// Queues a game update object after serializing and encapsulating.
        /// Use this function to queue a game update object to send to
        /// clients.
        /// 
        /// TODO: replace gUpdate type with the actual game update type.
        /// 
        /// Author: Jeremy L
        /// </summary>
        /// <param name="gUpdate">game update object</param>
        /// <returns>true if queuing is successful</returns>
        public bool QueueUpdate(Object gUpdate)
        {
            byte[] buffSnd = new byte[BUFF_SIZE]; // buffer for data to send

            // serialize game update object
            buffSnd = serializeUpdate(gUpdate);
            if (buffSnd == null) // SerializeUpdate failed
                return false;

            // encapsulate serialized game update
            buffSnd = encapsulateUpdate(buffSnd);
            if (buffSnd == null) // EncapsulateUpdate failed
                return false;

            this.updateQueueToSend.Enqueue(buffSnd); // queue converted game update

            return true; // Queuing sueccessful
        }

        /// <summary>
        /// A different version of QueueUpdate -- takes byte array instead.
        /// This function is for MVP and to be removed afterwards.
        /// 
        /// TODO: delete when we are using game update objects instead
        ///       of byte array.
        /// 
        /// Author: Jeremy L
        /// </summary>
        /// <param name="gUpdate">game update byte array</param>
        /// <returns>true if queuing is successful</returns>
        public bool QueueUpdate(byte[] gUpdate)
        {
            this.updateQueueToSend.Enqueue(gUpdate); // queue converted game update

            return true; // Queuing sueccessful
        }

        /// <summary>
        /// Driver to send a game update object to all clients. Specify
        /// the condition of connections to send the game update to.
        /// 
        /// TODO: switch the parameter type of write to byte[] in
        ///       Connection class
        /// 
        /// Author: Jeremy L
        /// </summary>
        private void sendUpdateToClients()
        {
            byte[] dequeued;
            
            while (this.runSender)
            {
                if (this.updateQueueToSend.TryDequeue(out dequeued))
                {
                    foreach (Connection conn in this.connPool)
                    {
                        if (true) // TODO: specify condition of connection
                        {
                            conn.WriteToBuffer(dequeued);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Serialize a game update object into a binary array.
        /// 
        /// TODO: replace gUpdate type with the actual game update type.
        /// 
        /// Author: Jeremy L
        /// </summary>
        /// <param name="gUpdate">game update object</param>
        /// <returns>serialized game update</returns>
        private byte[] serializeUpdate(Object gUpdate)
        {
            if (gUpdate == null) // null check
                return null;

            byte[] buffBin;
            // initialize a game update serializer
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            // serialize game update object
            bf.Serialize(ms, gUpdate);
            // assign serialized game update object to buff
            buffBin = ms.ToArray();

            return buffBin;
        }

        /// <summary>
        /// Encapsulates the serialized game update.
        /// 
        /// TODO: encapsulation is not to be implemented for MVP.
        /// 
        /// Author: Jeremy L
        /// </summary>
        /// <param name="buff">serialized game update</param>
        /// <returns>encapsulated game update</returns>
        private byte[] encapsulateUpdate(byte[] buff)
        {
            if (buff == null) // null check
                return null;

            byte[] buffEncap = null;

            // TODO: initialize an encoding format

            // TODO: convert format of the buffer content

            // TEMP code for MVP
            buffEncap = buff;

            return buffEncap;
        }
        #endregion // Sender -------------------------------------------

        #region // Receiver --------------------------------------------
        /// <summary>
        /// Starts the receiver on a separate thread. Use this function
        /// to start processing received datagrams in datagram queue.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public void StartReceiver()
        {
            this.runReceiver = false;
            this.thrDatagramRcvr = new Thread(receiveFromClient);
            this.thrRxQueueReader = new Thread(readFromRcvdQueue);
            this.thrRxQueueReader.Start();
        }

        /// <summary>
        /// Stops the receiver thread.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public void StopReceiver()
        {
            this.runReceiver = false;
            Thread.Sleep(1000); // sleep for cleaning up
            this.thrDatagramRcvr = null;
            this.thrRxQueueReader = null;
        }

        /// <summary>
        /// Continuously receives datagram from all clients and queue
        /// them.
        /// 
        /// Author: Jeremy L
        /// </summary>
        private void receiveFromClient()
        {
            byte[] buffRcvd;

            while (runReceiver)
            {
                // TODO: use low level function to read from all connections
                foreach (Connection conn in connPool)
                {
                    buffRcvd = new byte[BUFF_SIZE];
                    if (conn.ReadFromBuffer(buffRcvd))
                    {
                        buffRcvd.CopyTo(buffRcvd, 0);
                        this.rcvdDatagramQueue.Enqueue(buffRcvd);
                    }
                    else
                    {
                        // TODO: log failure
                    }
                }
            }
        }

        /// <summary>
        /// Receives datagrams from clients.
        /// 
        /// TODO: implement details.
        /// 
        /// Author: Jeremy L
        /// </summary>
        private void readFromRcvdQueue()
        {
            int dataType;
            byte[] datagram;

            while (this.runReceiver)
            {
                // Receive from all clients
                if (rcvdDatagramQueue.TryDequeue(out datagram))
                {
                    // TODO: parse the part that indicates data type from
                    //       the datagram
                    //dataType = datagram. ...

                    // TEMP: parsing data type
                    dataType = 0;

                    switch (dataType)
                    {
                        case 0: // TODO: replace the value for Payload

                            // TODO: check status of connection
                            //       if connecting, change to connected

                            // TODO: update seq

                            // decapsulate the datagram
                            byte[] buffRx = decapsulateDatagram(datagram);
                            if (buffRx == null) // decapsulation failed
                            {
                                // TODO: log failure
                                break;
                            }

                            // deserialize data into game update object
                            Object gUpdate = deserializeDatagram(buffRx);
                            if (gUpdate == null) // deserialization failed
                            {
                                // TODO: log failure
                                break;
                            }

                            // TODO: delegate to game controller

                            break;
                        case 1: // TODO: replace the value for Connection
                            // check number of clients
                            if (this.connPool.Count < MAX_CLIENTS) // not full
                            {
                                // TODO: add client to list

                                // TODO: send Challenge packet

                                // TODO: change status of the new client to
                                //       pending challenge
                            }
                            else // capacity full
                            {
                                // ignore connection request
                            }

                            break;
                        case 2: // TODO: replace the value for Challenging Response
                                // TODO: check validity of challenge
                                //       If valid, change connection status
                                //       to connecting

                            // TODO: send response packet

                            // TODO: update seq number

                            break;
                        case 3: // TODO: replace the value for Keeping Alive
                            // TODO: check status of connection
                            //       If connecting, change to connected
                            //       Else, reset connection timeout and
                            //       update seq

                            break;
                        case 4: // TODO: replace the value for Disconnection
                            // TODO: check if client is currently connected
                            //       If so, remove the client from pool
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Decapsulates received datagram
        /// 
        /// Author: Jeremy L
        /// </summary>
        /// <param name="buff">raw datagram</param>
        /// <returns>decapsulated datagram</returns>
        private byte[] decapsulateDatagram(byte[] buff)
        {
            if (buff == null)
            {
                return null;
            } // null check


            byte[] buffDecap = null;

            // TODO: initialize a decoder

            // TODO: decapsulate datagram

            // TEMP code for MVP
            buffDecap = buff;

            return buffDecap;
        }

        /// <summary>
        /// Deserializes the decapsulated datagram
        /// 
        /// Author: Jeremy L
        /// </summary>
        /// <param name="buff">decapsulated datagram</param>
        /// <returns>decapsulated game update object</returns>
        private Object deserializeDatagram(byte[] buff)
        {
            if (buff == null)
            {
                return null;
            }

            Object gUpdate = null;

            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(buff, 0, buff.Length);
            ms.Seek(0, SeekOrigin.Begin);
            gUpdate = (Object)bf.Deserialize(ms);

            return gUpdate;
        }
        #endregion // --------------------------------------------------
    }
}