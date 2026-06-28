namespace UrbanNoodle.Repository.Interface
{
    public interface IKnowledgeChunksRepository
    {
        Task<List<string>> SearchSimilarContextAsync(float[] queryVector);
    }
}
