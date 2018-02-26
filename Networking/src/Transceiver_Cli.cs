using System;
using System.Collections.Concurrent; // CocurrentQueue
using System.Collections.Generic; // List
using System.IO; // MemoryStream
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter
using System.Threading;

namespace COMP4981_NetworkingTest
{
    /// <summary>
    /// Client driver to send and receive game updates.
    /// 
    /// Author: Jeremy L
    /// </summary>
    public class Transceiver_Cli
    {
        private const int BUFF_SIZE = 1200; // buffer size
        private const int MAX_CLIENTS = 30; // max number of client conns

        // Sender and Receiver
        private Thread thrSender;
        private Thread thrReceiver;
        private bool runSender;
        private bool runReceiver;
        // data structures
        private ConcurrentQueue<byte[]> updateQueueToSend;
        private ConcurrentQueue<byte[]> rcvdDatagramQueue; // rx datagram queue
        private Connection connToSrv; // connection to server

        /// <summary>
        /// Constructor for a Transceiver object.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public Transceiver_Cli()
        {
            this.connToSrv = new Connection();
            this.runSender = false;
            this.runReceiver = false;
            this.updateQueueToSend = new ConcurrentQueue<byte[]>();
            this.rcvdDatagramQueue = new ConcurrentQueue<byte[]>();
        }

        ~Transceiver_Cli()
        {
            StopSender();
            StopReceiver();
        }

        #region // Transceiver -----------------------------------------
        /// <summary>
        /// Note that this function basically abandon the current Connection
        /// and creates a new one.
        /// 
        /// Author: Willson Hu, Jeremy L
        /// </summary>
        public void CreateConnection()
        {
            this.connToSrv = new Connection();
        }

        /// <summary>
        /// 
        /// Author: Willson Hu
        /// </summary>
        public bool RemoveConnection()
        {
            return false;
        }
        #endregion // Transceiver --------------------------------------

        #region // Sender ----------------------------------------------
        /// <summary>
        /// Starts Sender on a separate thread. Use this function to
        /// start sending queued game updates to server.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public void StartSender()
        {
            this.runSender = true;
            this.thrSender = new Thread(sendUpdateToServer);
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
        /// server.
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
        /// Driver to send a game update object to server. Specify
        /// 
        /// TODO: switch the parameter type of write to byte[] in
        ///       Connection class
        /// 
        /// Author: Jeremy L
        /// </summary>
        private void sendUpdateToServer()
        {
            byte[] dequeued;

            while (this.runSender)
            {
                if (this.updateQueueToSend.TryDequeue(out dequeued))
                {
                    connToSrv.WriteToBuffer(dequeued);
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
            this.thrReceiver = new Thread(receiveDatagramFromClients);
            this.thrReceiver.Start();
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
            this.thrReceiver = null;
        }

        /// <summary>
        /// Receives datagrams from clients.
        /// 
        /// TODO: implement details.
        /// 
        /// Author: Jeremy L
        /// </summary>
        private void receiveDatagramFromClients()
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