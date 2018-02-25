using System;
using System.Collections.Concurrent; // CocurrentQueue
using System.Collections.Generic; // List
using System.IO; // MemoryStream
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter
using System.Threading;

namespace Networking
{
    /// <summary>
    /// Driver to send and receive game updates.
    /// 
    /// author: Jeremy L
    /// </summary>
    public class Tranceiver
    {
        private const int BUFF_SIZE = 128; // buffer size
        private const int MAX_CLIENT = 64; // max number of client conns

        // Sender and Receiver
        private Thread thrSender;
        private Thread thrReceiver;
        private bool runSender;
        private bool runReceiver;
        // data structures
        private ConcurrentQueue<byte[]> updateQueueToSend;
        private ConcurrentQueue<byte[]> datagramQueue; // rx datagram queue
        private List<Connection> connPool; // connection pool

        /// <summary>
        /// Constructor for a Tranceiver object
        /// 
        /// author: Jeremy L
        /// </summary>
        public Tranceiver(ref List<Connection> connPool, ref ConcurrentQueue<byte[]> datagramQueue)
        {
            this.runSender = false;
            this.runReceiver = false;
            this.updateQueueToSend = new ConcurrentQueue<byte[]>();
            this.datagramQueue = datagramQueue;
            this.connPool = connPool;
        }

        ~Tranceiver()
        {
            StopSender();
            StopReceiver();
        }

        #region // Sender
        /// <summary>
        /// Starts Sender on a separate thread. Use this function to
        /// start sending queued game updates to all clients.
        /// 
        /// author: Jeremy L
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
        /// author: Jeremy L
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
        /// TODO: replace gUpdate type with the actual game update type
        /// 
        /// author: Jeremy L
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
        /// Driver to send a game update object to all clients. Specify
        /// the condition of connections to send the game update to.
        /// 
        /// TODO: switch the parameter type of write to byte[] in
        ///       Connection class
        /// 
        /// author: Jeremy L
        /// </summary>
        private void sendUpdateToClients()
        {
            while (this.runSender)
            {
                if (this.updateQueueToSend.TryDequeue(out byte[] dequeued))
                {
                    foreach (Connection conn in this.connPool)
                    {
                        if (true) // TODO: specify condition of connection
                        {
                            conn.write(dequeued);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Serialize a game update object into a binary array.
        /// 
        /// TODO: replace gUpdate type with the actual game update type
        /// 
        /// author: Jeremy L
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
        /// author: Jeremy L
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
        #endregion

        #region // Receiver
        /// <summary>
        /// Starts the receiver on a separate thread. Use this function
        /// to start processing received datagrams in datagram queue.
        /// 
        /// author: Jeremy L
        /// </summary>
        public void StartReceiver()
        {
            this.runReceiver = false;
            this.thrReceiver = new Thread(receiveDatagramFromClients);
            this.thrReceiver.Start();
        }

        /// <summary>
        /// Stops the receiver thread.
        /// 
        /// author: Jeremy L
        /// </summary>
        public void StopReceiver()
        {
            this.runReceiver = false;
            Thread.Sleep(1000); // sleep for cleaning up
            this.thrReceiver = null;
        }

        /// <summary>
        /// Receives datagrams from clients.
        /// 
        /// TODO: implement details.
        /// 
        /// author: Jeremy L
        /// </summary>
        private void receiveDatagramFromClients()
        {
            int dataType;

            while (this.runReceiver)
            {
                // Receive from all clients
                if (datagramQueue.TryDequeue(out byte[] datagram))
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
                            if (this.connPool.Count < MAX_CLIENT) // not full
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
        /// author: Jeremy L
        /// </summary>
        /// <param name="buff">raw datagram</param>
        /// <returns>decapsulated datagram</returns>
        private byte[] decapsulateDatagram(byte[] buff)
        {
            if (buff == null) // null check
                return null;

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
        /// author: Jeremy L
        /// </summary>
        /// <param name="buff">decapsulated datagram</param>
        /// <returns>decapsulated game update object</returns>
        private Object deserializeDatagram(byte[] buff)
        {
            if (buff == null)
                return null;

            Object gUpdate = null;

            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(buff, 0, buff.Length);
            ms.Seek(0, SeekOrigin.Begin);
            gUpdate = (Object)bf.Deserialize(ms);

            return gUpdate;
        }
        #endregion
    }
}