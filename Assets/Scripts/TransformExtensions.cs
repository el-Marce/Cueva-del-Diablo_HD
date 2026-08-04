using UnityEngine;

// Extensión para obtener la ruta completa de un Transform en la jerarquía.
// Usada como ID estable y único para pickups entre recargas de escena.
public static class TransformExtensions
{
    public static string GetFullPath(this Transform transform)
    {
        string path = transform.name;
        Transform current = transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return path;
    }
}
