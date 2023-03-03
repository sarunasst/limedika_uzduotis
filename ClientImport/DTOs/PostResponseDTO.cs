namespace ClientImport.DTOs
{
    public class PostResponseDTO
    {
        public string status { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public int message_code { get; set; }
        public int total { get; set; }
        public PostResponseResultDTO[] data { get; set; }
    }
}
