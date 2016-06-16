using System.IO;

namespace Ober.Test.Extensions
{
    public static class PathExtensions
    {
        public static string GetExecutingPath(this object executingObject)
        {
            var type = executingObject.GetType();
            //get the full location of the assembly with DaoTests in it
            var fullPath = System.Reflection.Assembly.GetAssembly(type).Location;
            //get the folder that's in
            return Path.GetDirectoryName(fullPath);
        }
    }
}
