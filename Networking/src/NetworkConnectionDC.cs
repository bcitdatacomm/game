
/**
Class skeleton for our implementation of Unity's Network Connection class.

Inner class PacketStat has not been added, can add if we need to track
the size / num / types of packets
*/
public class NetworkConnectionDC
{
    //Transport layer host ID for this connection
    private int hostId = -1;
    //Unique identifier for this connection
    private connectionId = -1;
    private NetworkWriter m_Writer;
    private isReady;
    //IP Address associated with the connection
    private string address;
    //Time last message was received on this connection
    public float lastMessageTime;
    private HashSet<NetworkInstanceId> m_ClientOwnedObjects;

    //Constructor
    public NetworkConnectionDC()
    {

    }

    //Destructor
    ~NetworkConnectionDC()
    {

    }

    //Initializer function, intitializes internal data structures of the object
    public virtual void Initialize(string networkAddress, int networkHostId, int networkConnecitonId, HostTopology hostTopology)
    {
        
    }

    //Spelled correctly.
    public virtual bool TransportReceive(byte[] bytes, int numBytes, int channelId, out byte error)
    {

    }

    public virtual bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
    {

    }

    //Called by TransportReceive
    //Calls HandleReader (Unity why.)
    protected void HandleBytes(byte[] buffer, int receivedSize, int channelId)
    {

    }

    //Makes connection process the data contained in the stream, and call handler functions
    protected void HandleReader(NetworkReader reader, int receivedSize, int channelId)
    {

    }

    //Disconnect this connection 
    public void Disconnect()
    {

    }

    public bool isConnected()
    {

    }

    public string ToString()
    {

    }
}