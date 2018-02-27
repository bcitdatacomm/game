using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using RunLibrary;


namespace COMP4981_NetworkingTest
{
    // TODO: Convert connectionStatus uint to enum
    enum connectionStatus : byte
    {
        disconnected = 0,
        connection_unstable = 1,
        connection_stable = 2,
    }

    /* SOURCE FILE: Connection
     * 
     * DATE:
     * 
     * FUNCTIONS:
     * 
     * DESIGNER: Delan Elliot, Jeffrey Chou, Jeremy Lee, Wilson Hu
     * 
     * PROGRAMMER: Wilson Hu
     * 
     * NOTES: Connection class which will be initialized by the Transceiver class
     *          of each instance of the game. A client instance (player) will have
     *          one Connection, while the server will store up to 30. 
     */
    public unsafe class Connection
    {

        
        public const ushort PORT_NO = 9999;
        public const Int32 SOCKET_DATA_WAITING = 1;
        public const Int32 SOCKET_NODATA = 0;


        private uint udpEndPointId;
        private uint connectionStatus = 0;


        private EndPoint endPoint;
        IntPtr serverInstance = Server.Server_CreateServer();

        private bool runRecvThread = false;
        private Thread recvThread;

        /* FUNCTION: Connection()
         * 
         * DATE:
         * 
         * DESIGNER: Delan Elliot, Jeffrey Chou, Jeremy Lee, Wilson Hu
         * 
         * PROGRAMMER: Wilson Hu
         * 
         * INTERFACE: uint udpEndPointId
         *          - Takes a udpEndPointId which is used to identify the client
         *            or server connection EndPoint.
         * 
         * RETURNS: Connection object(?)
         * 
         * NOTES: Connection class constructor.
         *          - This constructor initializes the input and output buffers
         *          - connectionStatus is initialized to 0 (disconnected)
         */
        public Connection(uint udpEndPointId)
        {
            this.udpEndPointId = udpEndPointId;
            this.ackNo = 0;
            this.connectionStatus = 0;

            this.inputBufPos = 0;
            this.ackNo = 0;
            this.endPoint = new EndPoint();
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
            if (input.Length + this.outputBufPos < MAX_BUF_SIZE)
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
            if (input.Length + this.inputBufPos < MAX_BUF_SIZE)     // This logic may not be necessary, as WriteToInputBuffer reads from input that comes from server
            {
                Buffer.BlockCopy(input, 0, this.inputBuffer, this.inputBufPos, input.Length);    // May refactor to ArrayCopy
                this.inputBufPos += input.Length;
                return true;
            }
            return false;
        }


        


        


        

        /* FUNCTION:
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
         */
        public unsafe byte[] GetInputBuffer() { return this.inputBuffer; }

        /* FUNCTION:
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
         */
        public unsafe byte[] GetOutputBuffer() { return this.outputBuffer; }

        /* FUNCTION:
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
         */
        public uint GetConnectionStatus() { return this.connectionStatus; }

        /* FUNCTION:
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
         */
        public void SetConnectionStatus(uint status) { this.connectionStatus = status; }




        /*  
         * 
         */
        //public void StartReceiving()
        //{
        //    this.recvThread = new Thread(recvThreadFunc);
        //}

        ///*
        // * 
        // */
        //private void recvThreadFunc()
        //{
        //    while (this.runRecvThread)
        //    {
                
        //    }
        //}
    }
}
