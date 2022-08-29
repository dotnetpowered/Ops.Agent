using System;

namespace Ops.Agents.NewRelic;

class Actor
{
    public EntitySearch? entitySearch { get; set; }
}

class Entity
{
    public string? entityType { get; set; }
    public string? guid { get; set; }
    public string? name { get; set; }
    public List<Tag>? tags { get; set; }
    public string GetTagValue(string key) => tags.Find(f => f.key == key)?.values?.FirstOrDefault();
    public double GetTagValueDouble(string key) => double.Parse(GetTagValue(key));
    public int GetTagValueInt(string key) => int.Parse(GetTagValue(key));
}

class EntitySearch
{
    public string? query { get; set; }
    public Results? results { get; set; }
}

class Results
{
    public List<Entity>? entities { get; set; }
}

class Tag
{
    public string? key { get; set; }
    public List<string>? values { get; set; }
}

class Response
{
    public Actor? actor { get; set; }
}

