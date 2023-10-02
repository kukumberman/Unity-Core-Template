namespace Game.Core
{
    public interface ISaveEncoderDecoder
    {
        string Encode(string data);

        string Decode(string data);
    }
}
