using System;
using System.IO; // MemoryStream
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter

namespace Networking
{
    public class Tranceiver
    {
        private const int BUFFSIZE = 128; // buffer size



        /// <summary>
        /// Constructor
        /// </summary>
        public Tranceiver()
        {

        }

        /// <summary>
        /// Driver for serializing, encapsulating, and sending a game
        /// update object to clients
        /// 
        /// TODO: replace gUpdate type with the actual game update type
        /// TODO: call the low level send function
        /// </summary>
        /// <param name="gUpdate">game update object</param>
        /// <returns>true if sending a game update is successful</returns>
        public bool SendUpdate(Object gUpdate)
        {
            byte[] buffSnd = new byte[BUFFSIZE]; // buffer for data to send

            // serialize game update object
            buffSnd = SerializeUpdate(gUpdate);
            if (buffSnd == null) // SerializeUpdate failed
                return false;

            // encapsulate serialized game update
            buffSnd = EncapsulateUpdate(buffSnd);
            if (buffSnd == null) // EncapsulateUpdate failed
                return false;

            // TODO: send converted game update


            return true; // SendUpdate successful
        }

        /// <summary>
        /// Serialize a game update object into a binary array.
        /// 
        /// TODO: replace gUpdate type with the actual game update type
        /// </summary>
        /// <param name="gUpdate"></param>
        /// <returns>serialized game update</returns>
        private byte[] SerializeUpdate(Object gUpdate)
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
        /// 
        /// </summary>
        /// <param name="buff"></param>
        /// <returns>encapsulated, serialized game update</returns>
        private byte[] EncapsulateUpdate(byte[] buff)
        {
            byte[] buffEncap = new byte[BUFFSIZE];

            // TODO: initialize an encoding format

            // TODO: convert format of the buffer content

            return buffEncap;
        }

        /// <summary>
        /// Receives game updates from clients
        /// 
        /// TODO: this is a skeleton function. Implement details.
        /// </summary>
        /// <returns></returns>
        public bool ReceiveUpdate()
        {

            return true;
        }
    }
}