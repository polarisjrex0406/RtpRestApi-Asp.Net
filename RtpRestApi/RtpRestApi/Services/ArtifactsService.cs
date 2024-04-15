using Microsoft.Extensions.Options;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using System.Text.Json.Serialization.Metadata;

namespace RtpRestApi.Services
{
    public class ArtifactsService
    {
        IAtlasService _atlasService;
        private readonly string _collection;

        public ArtifactsService(IOptions<RtpServerSettings> rtpDatabaseTopics, IAtlasService atlasService)
        {
            _atlasService = atlasService;
            _collection = rtpDatabaseTopics.Value.ArtifactsCollectionName;
        }

        private JsonSerializerOptions? SerializeOptions()
        {
            JsonSerializerOptions? options;
            try
            {
                options = new JsonSerializerOptions
                {
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver
                    {
                        Modifiers =
                    {
                        static typeInfo =>
                        {
                            if (typeInfo.Kind != JsonTypeInfoKind.Object)
                                return;

                            bool flagPassed = false;
                            foreach (JsonPropertyInfo propertyInfo in typeInfo.Properties)
                            {
                                // Strip IsRequired constraint from every property.
                                if (!flagPassed && propertyInfo.Name == "topic")
                                {
                                    propertyInfo.Name = "topicObj";
                                    flagPassed = true;
                                }
                                if (propertyInfo.Name == "topicId")
                                {
                                    propertyInfo.Name = "topic";
                                }
                            }
                        }
                    }
                    }
                };
            }
            catch (Exception)
            {
                return null;
            }
            return options;
        }

        public async Task<List<ArtifactResponse>?> GetAsync(string? adminId = null, string? q = null, string? fields = null)
        {
            JArray andArray = new JArray();
            JObject removed = new JObject
            {
                ["removed"] = false
            };
            andArray.Add(removed);

            if (adminId != null)
            {
                JObject createdBy = new JObject
                {
                    ["createdBy"] = new JObject
                    {
                        ["$oid"] = adminId
                    }
                };
                andArray.Add(createdBy);
            }

            if (q != null && fields != null)
            {
                List<string> fieldsList = new List<string>(fields.Split(','));
                JArray objArray = new JArray();
                foreach (string field in fieldsList)
                {
                    objArray.Add(new JObject
                    {
                        [$"{field}"] = new JObject
                        {
                            ["$regex"] = q,
                            ["$options"] = "i"
                        }
                    });
                }
                andArray.Add(new JObject
                {
                    ["$or"] = objArray
                });
            }

            JObject filterObj = new JObject
            {
                ["$and"] = andArray
            };

            string res = await _atlasService.FindAsync(_collection, filterObj);
            var expObj = new List<ArtifactResponse>();
            try
            {
                expObj = JsonSerializer.Deserialize<List<ArtifactResponse>>(res, SerializeOptions());
            }
            catch (Exception)
            {
                return new List<ArtifactResponse>();
            }

            return expObj;
        }

        public async Task<ArtifactResponse?> GetAsync(string? adminId, string? id)
        {
            JObject filterObj = new JObject();
            filterObj["removed"] = false;
            if (adminId != null)
            {
                JObject createdBy = new JObject
                {
                    ["$oid"] = adminId
                };
                filterObj["createdBy"] = createdBy;
            }
            if (id != null)
            {
                JObject _id = new JObject
                {
                    ["$oid"] = id
                };
                filterObj["_id"] = _id;
            }

            string res = await _atlasService.FindOneAsync(_collection, filterObj);
            var expObj = new ArtifactResponse();
            try
            {
                expObj = JsonSerializer.Deserialize<ArtifactResponse>(res, SerializeOptions());
            }
            catch (Exception)
            {
                return null;
            }

            return expObj;
        }

        public async Task<ArtifactResponse?> CreateAsync(string? adminId, ArtifactRequest newArtifactRequest)
        {
            ArtifactResponse artifactResponse = new ArtifactResponse();
            artifactResponse.name = newArtifactRequest.name;
            artifactResponse.goal = newArtifactRequest.goal;
            artifactResponse.group = newArtifactRequest.group;
            artifactResponse.promptEnhancers = newArtifactRequest.promptEnhancers;
            artifactResponse.promptOutput = newArtifactRequest.promptOutput;
            artifactResponse.examples = newArtifactRequest.examples;
            artifactResponse.chatgptSettings = newArtifactRequest.chatgptSettings;
            artifactResponse.useCache = newArtifactRequest.useCache;
            artifactResponse.cacheTimeoutUnit = newArtifactRequest.cacheTimeoutUnit;
            artifactResponse.cacheTimeoutValue = newArtifactRequest.cacheTimeoutValue;
            artifactResponse.cacheConditions = newArtifactRequest.cacheConditions;
            artifactResponse.cacheDescription = newArtifactRequest.cacheDescription;
            artifactResponse.createdBy = adminId;
            string tmp = JsonSerializer.Serialize(artifactResponse, SerializeOptions());
            JObject documentObj = JObject.Parse(tmp);
            documentObj.Remove("_id");
            documentObj.Remove("topicObj");
            documentObj.Remove("createdBy");
            documentObj["createdBy"] = new JObject
            {
                ["$oid"] = adminId
            };
            documentObj["topic"] = new JObject
            {
                ["$oid"] = newArtifactRequest.topic
            };

            string res = await _atlasService.InsertOneAsync(_collection, documentObj);
            try
            {
                var resObj = JObject.Parse(res);
                var idObj = resObj["insertedId"];
                artifactResponse._id = idObj?.ToString();
            }
            catch (Exception)
            {
                return null;
            }

            return artifactResponse;
        }

        public async Task<ArtifactResponse?> UpdateAsync(string id, ArtifactRequest updatedArtifact)
        {
            JObject filterObj = new JObject
            {
                ["_id"] = new JObject
                {
                    ["$oid"] = id
                }
            };
            string tmp = JsonSerializer.Serialize(updatedArtifact);
            JObject setObj = JObject.Parse(tmp);

            string res = await _atlasService.UpdateOneAsync(_collection, filterObj, setObj);
            int matchedCount = 0;
            try
            {
                var resObj = JObject.Parse(res);
                var idObj = resObj["matchedCount"];
                if (idObj != null)
                {
                    matchedCount = int.Parse(idObj.ToString());
                }
            }
            catch (Exception)
            {
                return null;
            }

            if (matchedCount > 0)
            {
                return new ArtifactResponse();
            }
            else
            {
                return null;
            }
        }

        public async Task<string> RemoveAsync(string id)
        {
            JObject filterObj = new JObject
            {
                ["_id"] = new JObject
                {
                    ["$oid"] = id
                }
            };
            return await _atlasService.DeleteOneAsync(_collection, filterObj);
        }
    }
}
