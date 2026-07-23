namespace UrbanNoodle.Repository.Interface
{
    public interface IKnowledgeChunksRepository
    {
        public Task<List<string>> SearchSimilarContextAsync(float[] queryVector);
    }
}
