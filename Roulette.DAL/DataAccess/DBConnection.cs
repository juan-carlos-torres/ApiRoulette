using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Roulette.DAL.DataAccessOptions;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Roulette.DAL.DataAccessDTO;

namespace Roulette.DAL.DataAccess
{
    public class DBConnection
    {
        private readonly IDatabase _dataBase;
        private readonly IServer _server;

        public DBConnection(IOptions<RedisConfigurationDTO> redis)
        {
            var connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redis.Value.Host));
            _dataBase = connection.Value.GetDatabase();
            var endpoints = _dataBase.Multiplexer.GetEndPoints();
            _server = _dataBase.Multiplexer.GetServer(endpoints.First());
        }

        public async Task<T> GetByKey<T>(string key)
        {
            try
            {
                var llave = new RedisKey(key);
                var value = await _dataBase.StringGetAsync(key);
                var mapValue = JsonConvert.DeserializeObject<T>(value.ToString());

                return mapValue;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        public async Task AddKey<T>(string key, T objectToSave)
        {
            try
            {
                var value = JsonConvert.SerializeObject(objectToSave);
                var newKey = new RedisKey(key);
                var valueNewKey = new RedisValue(value);
                var responseSave = await _dataBase.StringAppendAsync(newKey, valueNewKey);
                if (responseSave == 0)
                {
                    throw new Exception("Ha ocurrido un error al guardar la llave");
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        public async Task UpdateKey<T>(string key, T objectToUpdate)
        {
            try
            {
                var value = JsonConvert.SerializeObject(objectToUpdate);
                var newKey = new RedisKey(key);
                var newKeyValue = new RedisValue(value);
                var responseSave = await _dataBase.StringSetAsync(newKey, newKeyValue);
                if (!responseSave)
                {
                    throw new Exception("Ha ocurrido un error al actualizar la llave");
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

        public async Task SetValueByKey(string key, string value)
        {
            try
            {
                var responseSave = await _dataBase.StringSetAsync(key, value);
                if (!responseSave)
                {
                    throw new Exception();
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }


        public async Task<List<RouletteDTO>> GetListAll()
        {
            try
            {
                var listKeys = _server
                                .Keys()
                                .Select(k => k.ToString())
                                .ToList();
                var roulettesList = new List<RouletteDTO>();
                foreach (string key in listKeys)
                {
                    roulettesList.Add(await GetByKey<RouletteDTO>(key));
                }

                return roulettesList;
            }
            catch (Exception err)
            {
                throw new Exception(err.ToString());
            }
        }

    }
}
