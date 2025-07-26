using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
namespace Lab6.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Comment { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string AnswersJson { get; set; }
        public string BadAnswersJson { get; set; }

        [NotMapped]
        public List<string> Answers
        {
            get => JsonSerializer.Deserialize<List<string>>(AnswersJson ?? "[]");
            set => AnswersJson = JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public List<string> BadAnswers
        {
            get => JsonSerializer.Deserialize<List<string>>(BadAnswersJson ?? "[]");
            set => BadAnswersJson = JsonSerializer.Serialize(value);
        }
    }

}
