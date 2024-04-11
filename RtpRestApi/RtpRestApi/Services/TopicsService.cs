﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using RtpRestApi.Helpers;
using RtpRestApi.Models;

namespace RtpRestApi.Services
{
    public class TopicsService
    {
        IAtlasService _atlasService;
        private readonly string _collection;

        public TopicsService(IOptions<RtpDatabaseSettings> rtpDatabaseTopics, IAtlasService atlasService)
        {
            _atlasService = atlasService;
            _collection = rtpDatabaseTopics.Value.TopicsCollectionName;
        }

        public async Task<List<TopicResponse>?> GetAsync(string? adminId = null, string? q = null, string? fields=null)
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
            var expObj = new List<TopicResponse>();

            try
            {
                expObj = JsonSerializer.Deserialize<List<TopicResponse>>(res);
            }
            catch (Exception ex)
            {
                return new List<TopicResponse>();
            }

            return expObj;
        }

        public async Task<TopicResponse?> GetAsync(string? adminId, string? id)
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
            var expObj = new TopicResponse();

            try
            {
                expObj = JsonSerializer.Deserialize<TopicResponse>(res);
            }
            catch (Exception ex)
            {
                return null;
            }

            return expObj;
        }

        public async Task<TopicResponse?> CreateAsync(string? adminId, TopicRequest newTopicRequest)
        {
            TopicResponse topicResponse = new TopicResponse();
            topicResponse.createdBy = adminId;
            topicResponse.name = newTopicRequest.name;
            topicResponse.goal = newTopicRequest.goal;
            topicResponse.group = newTopicRequest.group;
            topicResponse.topicPrompt = newTopicRequest.topicPrompt;
            string tmp = JsonSerializer.Serialize(topicResponse);
            JObject documentObj = JObject.Parse(tmp);
            documentObj.Remove("_id");
            
            string res = await _atlasService.InsertOneAsync(_collection, documentObj);

            try
            {
                var resObj = JObject.Parse(res);
                var idObj = resObj["insertedId"];
                string insertedId = idObj != null ? idObj.ToString() : string.Empty;
                topicResponse._id = insertedId;
            }
            catch (Exception ex)
            {
                return null;
            }

            return topicResponse;
        }

        public async Task UpdateAsync(string id, TopicRequest updatedTopic)
        {
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
            string res = await _atlasService.DeleteOneAsync(_collection, filterObj);
            return res;
        }
    }
}