namespace Gestion_de_Bibliothéque.DTOs
{
    public class UpdateBookDTOs
    {
        public string? Title { get; set; }  //Pour en lever les warning
        public string? Author { get; set; }    //on fait un point d'iterrogation apres le type
        public DateOnly PubDate { get; set; }
    }
}