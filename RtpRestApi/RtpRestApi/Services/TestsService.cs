using Microsoft.Extensions.Options;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using RtpRestApi.Helpers;
using RtpRestApi.Models;
using System.Text.Json.Serialization.Metadata;

namespace RtpRestApi.Services
{
    public class TestsService
    {
        IAtlasService _atlasService;
        private readonly string _collection;

        public TestsService(IOptions<RtpDatabaseSettings> rtpDatabaseTopics, IAtlasService atlasService)
        {
            _atlasService = atlasService;
            _collection = rtpDatabaseTopics.Value.TestsCollectionName;
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

                            bool flagTopic = false;
                            bool flagTemplate = false;
                            bool flagExp = false;
                            foreach (JsonPropertyInfo propertyInfo in typeInfo.Properties)
                            {
                                // Strip IsRequired constraint from every property.
                                if (!flagTopic && propertyInfo.Name == "topic")
                                {
                                    propertyInfo.Name = "topicObj";
                                    flagTopic = true;
                                }
                                if (propertyInfo.Name == "topicId")
                                {
                                    propertyInfo.Name = "topic";
                                }

                                if (!flagTemplate && propertyInfo.Name == "templateCode")
                                {
                                    propertyInfo.Name = "templateObj";
                                    flagTemplate = true;
                                }
                                if (propertyInfo.Name == "templateId")
                                {
                                    propertyInfo.Name = "templateCode";
                                }

                                if (!flagExp && propertyInfo.Name == "experiment")
                                {
                                    propertyInfo.Name = "experimentNoUse";
                                    flagExp = true;
                                }
                                if (propertyInfo.Name == "experimentNo")
                                {
                                    propertyInfo.Name = "experiment";
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

        public async Task<List<TestResponse>?> GetAsync(string? adminId = null, string? q = null, string? fields = null)
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
            var expObj = new List<TestResponse>();
            try
            {
                expObj = JsonSerializer.Deserialize<List<TestResponse>>(res, SerializeOptions());
            }
            catch (Exception)
            {
                return new List<TestResponse>();
            }

            return expObj;
        }

        public async Task<TestResponse?> GetAsync(string? adminId, string? id)
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
            var expObj = new TestResponse();
            try
            {
                expObj = JsonSerializer.Deserialize<TestResponse>(res, SerializeOptions());
            }
            catch (Exception)
            {
                return null;
            }

            return expObj;
        }

        public async Task<ExperimentResponse?> CreateAsync(string? adminId, ExperimentRequest newExperimentRequest)
        {
            ExperimentResponse experimentResponse = new ExperimentResponse();
            experimentResponse.experimentCode = newExperimentRequest.experimentCode;
            experimentResponse.description = newExperimentRequest.description;
            experimentResponse.style = newExperimentRequest.style;
            experimentResponse.initPrompt = newExperimentRequest.initPrompt;
            experimentResponse.topicId = newExperimentRequest.topic;
            experimentResponse.ruleLogic = newExperimentRequest.ruleLogic;
            experimentResponse.rules = newExperimentRequest.rules;
            experimentResponse.createdBy = adminId;
            string tmp = JsonSerializer.Serialize(experimentResponse, SerializeOptions());
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
                ["$oid"] = newExperimentRequest.topic
            };
            if (newExperimentRequest.templates != null && newExperimentRequest.templates.Count > 0)
            {
                JArray jArray = new JArray();
                foreach (var template in newExperimentRequest.templates)
                {
                    jArray.Add(new JObject
                    {
                        ["order"] = template.order,
                        ["templateCode"] = new JObject
                        {
                            ["$oid"] = template.templateCode
                        }
                    });
                }
                documentObj["templates"] = jArray;
            }

            string res = await _atlasService.InsertOneAsync(_collection, documentObj);
            try
            {
                var resObj = JObject.Parse(res);
                var idObj = resObj["insertedId"];
                experimentResponse._id = idObj?.ToString();
                if (newExperimentRequest.templates != null && newExperimentRequest.templates.Count > 0)
                {
                    experimentResponse.templates = new List<TemplateInResponse>();
                    foreach (var template in newExperimentRequest.templates)
                    {
                        TemplateInResponse tmpInRes = new TemplateInResponse();
                        tmpInRes.order = template.order;
                        tmpInRes.templateId = template.templateCode;
                        experimentResponse.templates.Add(tmpInRes);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return experimentResponse;
        }

        public async Task<TestResponse?> UpdateAsync(string id, TestRequest updatedTest)
        {
            JObject filterObj = new JObject
            {
                ["_id"] = new JObject
                {
                    ["$oid"] = id
                }
            };
            string tmp = JsonSerializer.Serialize(updatedTest);
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
                return new TestResponse();
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
