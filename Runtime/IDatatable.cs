namespace Drifter
{
    public interface IDatatable
    {
        public void Load(FileData data);
        public FileData Save();
    }
}