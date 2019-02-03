namespace Core.Entities.Interview
{
    public class Answer
    {
        public Question Question { get; }
        public string Content { get; }

        public Answer(Question question, string content)
        {
            Question = question;
            Content = content;
        }
    }
}