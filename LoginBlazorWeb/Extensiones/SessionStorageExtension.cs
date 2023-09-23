using Blazored.SessionStorage;
using System.Text.Json;

namespace LoginBlazorWeb.Extensiones
{
    public static class SessionStorageExtension
    {
        // Guardar token en el Storage
        public static async Task GuardarStorage<T>(this ISessionStorageService sessionStorageService,
            string key, T item)where T : class
        { 
            var item_json = JsonSerializer.Serialize(item);
            // guardamos el json con su key (clave:valor)
            await sessionStorageService.SetItemAsStringAsync(key, item_json);

        }

        // Obtener token del Storage
        public static async Task<T?> ObtenerStorage<T>(this ISessionStorageService sessionStorageService,
            string key) where T : class
        {
            var item_json = await sessionStorageService.GetItemAsStringAsync(key);
            if (item_json != null)
            {
                var item = JsonSerializer.Deserialize<T>(item_json);
                return item;
            }
            return null;
        }

    }
}
