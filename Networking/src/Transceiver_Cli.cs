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
    public unsafe class Transceiver_Cli
    {

        //public const int MAX_BUF_SIZE = 2048;
        private const int BUFF_SIZE = 1200; // buffer size
        private const int MAX_CLIENTS = 30; // max number of client conns
        public const Int32 SOCKET_DATA_WAITING = 1;
        public const Int32 SOCKET_NODATA = 0;

        // Sender and Receiver
        private Thread thrSender;
        private Thread thrDatagramRcvr;
        private Thread thrRxQueueReader;
        private bool runSender;
        private bool runReceiver;
        // data structures
        private ConcurrentQueue<byte[]> updateQueueToSend;
        private ConcurrentQueue<byte[]> rcvdDatagramQueue; // rx datagram queue

        private byte[] inputBuffer;
        private byte[] outputBuffer;
        private int inputBufPos;    // Not necessary? 
        private int outputBufPos;

        private IntPtr serverInstance;

        private uint ackNo;


        private Connection connToSrv; // connection to server

        /// <summary>
        /// Constructor for a Transceiver object.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public Transceiver_Cli()
        {
            this.connToSrv = new Connection(1);
            this.runSender = false;
            this.runReceiver = false;
            this.updateQueueToSend = new ConcurrentQueue<byte[]>();
            this.rcvdDatagramQueue = new ConcurrentQueue<byte[]>();

            this.inputBuffer = new byte[BUFF_SIZE];
            this.outputBuffer = new byte[BUFF_SIZE];

            serverInstance = Server.Server_CreateServer();


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
            this.connToSrv = new Connection(1);
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

        /* FUNCTION: Transmit()
         * 
         * DATE:
         * 
         * DESIGNER: Delan Elliot, Jeffrey Chou, Jeremy Lee, Wilson Hu
         * 
         * PROGRAMMER: Wilson Hu
         * 
         * INTERFACE: 
         * 
         * RETURNS:
         * 
         * NOTES:
         *
         * Transmit method, writes the contents of outputBuffer to the server by calling the Server_sendBytes library function.
         * TODO: Test functionality of writing ~1200 bytes each call, as well as int->uint cast.
         */
        public void Transmit()
        {
            fixed (byte* tempBuf = outputBuffer)
            {
                // TODO: Add reference to Connection's endpoint struct
                Server.Server_sendBytes(serverInstance, endPoint, new IntPtr(tempBuf), (uint)outputBuffer.Length);
            }
        }

        /* FUNCTION: WriteToOutputBuffer
         * 
         * DATE:
         * 
         * DESIGNER: Delan Elliot, Jeffrey Chou, Jeremy Lee, Wilson Hu
         * 
         * PROGRAMMER: Wilson Hu
         * 
         * INTERFACE: byte[] output: output buffer to be sent by the Connection object
         * 
         * RETURNS:
         * 
         * NOTES: This function takes a complete output buffer (complete game state update rather than append)
         *          and writes the contents to the Connection's output buffer, to be sent to socket.
         *
         * TODO: Refactor to WriteToOutputBuffer, writes output to buffer for server / client
         */
        public void WriteToOutputBuffer(byte[] output)
        {
            //Array.Clear(this.buffer, 0, this.buffer.Length);  //This part doesn't make sense. Currently clears buffer to be available to be written to. 
            Buffer.BlockCopy(this.outputBuffer, 0, output, 0, output.Length); //dest: otherBuffer, src: this.buffer;    --May Refactor to ArrayCopy
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
            this.runReceiver = true;
            this.thrDatagramRcvr = new Thread(receiveFromClient);
            //this.thrRxQueueReader = new Thread(readFromRcvdQueue);
            this.thrDatagramRcvr.Start();
            //this.thrRxQueueReader.Start();
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
        /// Continuously receives datagram from server and queue them.
        /// 
        /// Author: Jeremy L
        /// </summary>
        private void receiveFromClient()
        {
            byte[] buffRcvd;

            while (this.runReceiver)
            {
                buffRcvd = new byte[BUFF_SIZE];
                // TODO: Woz - Fix this or something.
                if (connToSrv.ReadFromBuffer(ref buffRcvd))
                {
                    this.rcvdDatagramQueue.Enqueue(buffRcvd);
                }
                else
                {
                    // TODO: log failure
                }
            }
        }

        /// <summary>
        /// Retrieves queued received messages and process them.
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
        /// For testing.
        /// 
        /// TODO: delete after test.
        /// 
        /// Author: Jeremy L
        /// </summary>
        public ConcurrentQueue<byte[]> GetRcvdDatagramQueue()
        {
            return rcvdDatagramQueue;
        }

        /// <summary>
        /// Decapsulates received datagram.
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
        /// Deserializes the decapsulated datagram.
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

        /* FUNCTION: ReadSocket()
         * 
         * DATE:
         * 
         * DESIGNER: Delan Elliot, Jeffrey Chou, Jeremy Lee, Wilson Hu
         * 
         * PROGRAMMER: Wilson Hu
         * 
         * INTERFACE:
         * 
         * RETURNS: bool indicating if data was read from the socket and stored in the input buffer
         * 
         * NOTES: 
         *
         * Reads socket for available data.
         * May require call to Marshal() to copy data from unmanaged to managed memory.
         */
        public bool ReadSocket()
        {
            Int32 result = Server.Server_PollSocket(serverInstance);

            if (result == SOCKET_DATA_WAITING)
            {
                fixed (byte* tempBuf = inputBuffer)
                {
                    // TODO: woz - fix endpoint and buf references
                    Int32 numBytesRecv = Server.Server_recvBytes(serverInstance, &endPoint, new IntPtr(tempBuf), 1400);
                    Array.Copy(tempBuf, inputBuffer, 1400);
                    return true;
                }

            }
            return false;
        }

        /* FUNCTION: AppendToOutputBuffer()
         * 
         * DATE:
         * 
         * DESIGNER: Delan Elliot, Jeffrey Chou, Jeremy Lee, Wilson Hu
         * 
         * PROGRAMMER: Wilson Hu
         * 
         * INTERFACE: byte[] input, short numBytes
         *          - input: byte array that holds game data to be appended to the output buffer
         *          - numBytes: number of bytes to be written to the outputBuffer
         * 
         * RETURNS: bool indicating whether append operation is successful.
         * 
         * NOTES:
         *
         *
         * TODO: Implement AppendToOutputBuffer, to add bytes to the end of the buffer
         */
        public unsafe bool AppendToOutputBuffer(byte[] input, short numBytes)
        {
            if (input.Length + this.outputBufPos < BUFF_SIZE)
            {
                Buffer.BlockCopy(input, 0, this.outputBuffer, this.outputBufPos, numBytes);    // May refactor to ArrayCopy
                this.outputBufPos += numBytes;
                return true;
            }
            return false;
        }


        /* FUNCTION: WriteToInputBuffer
         * 
         * DATE:
         * 
         * DESIGNER: Delan Elliot, Jeffrey Chou, Jeremy Lee, Wilson Hu
         * 
         * PROGRAMMER: Wilson Hu
         * 
         * INTERFACE: byte[] input: input buffer to be read from the socket. 
         * 
         * RETURNS: bool indicating if write operation is successful.
         * 
         * NOTES:
         *
         */
        public unsafe bool WriteToInputBuffer(byte[] input) //This is now read from buffer. It makes sense, kind of. It used to be write()
        {
            if (input.Length + this.inputBufPos < BUFF_SIZE)     // This logic may not be necessary, as WriteToInputBuffer reads from input that comes from server
            {
                Buffer.BlockCopy(input, 0, this.inputBuffer, this.inputBufPos, input.Length);    // May refactor to ArrayCopy
                this.inputBufPos += input.Length;
                return true;
            }
            return false;
        }
        #endregion // --------------------------------------------------
    }
}