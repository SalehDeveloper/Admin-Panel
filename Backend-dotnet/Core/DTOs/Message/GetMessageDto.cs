namespace Backend_dotnet.Core.DTOs.Message
{
    public class GetMessageDto
    { 
        public long Id { get; set; }    

        public string SenderName { get; set; }

        public string ReceiverName {  get; set; }   

        public string Text {  get; set; }   

        public DateTime CreatedAt { get; set; }   = DateTime.Now;
    }
}
