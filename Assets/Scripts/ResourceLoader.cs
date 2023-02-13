using UnityEngine;
using System.Linq;

public static class ResourceLoader
{
    public static T[] LoadResources<T>(string Path) where T : class {
        T[] list = Resources.LoadAll(Path, typeof(T)).Cast<T>().ToArray();
        return list;
    } 

    public static T LoadResource<T>(string Path) where T : class {
        T obj = Resources.Load(Path, typeof(T)) as T;
        return obj;
    } 
}