using System;
using Newtonsoft.Json;

namespace Ops.Agents.Elasticsearch.Models;


public class EsNode
{
    public string name { get; set; }
    public string transport_address { get; set; }
    public string host { get; set; }
    public string ip { get; set; }
    public string version { get; set; }
    public string build { get; set; }
    public string http_address { get; set; }
    public Settings settings { get; set; }
    public Os os { get; set; }
    public Process process { get; set; }
    public Jvm jvm { get; set; }
    public ThreadPool thread_pool { get; set; }
    public Transport transport { get; set; }
    public Http http { get; set; }
    //public List<dynamic> plugins { get; set; }
    public List<Module> modules { get; set; }
}

public class Http
{
    public string port { get; set; }
    public List<string> bound_address { get; set; }
    public string publish_address { get; set; }
    public int max_content_length_in_bytes { get; set; }
}

public class Node
{
    public string name { get; set; }
}

public class Index
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public int queue_size { get; set; }
}

public class Jvm
{
    public int pid { get; set; }
    public string version { get; set; }
    public string vm_name { get; set; }
    public string vm_version { get; set; }
    public string vm_vendor { get; set; }
    public long start_time_in_millis { get; set; }
    public Mem mem { get; set; }
    public List<string> gc_collectors { get; set; }
    public List<string> memory_pools { get; set; }
    public string using_compressed_ordinary_object_pointers { get; set; }
}

public class Listener
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public int queue_size { get; set; }
}

public class Management
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public string keep_alive { get; set; }
    public int queue_size { get; set; }
}

public class Mem
{
    public long heap_init_in_bytes { get; set; }
    public int heap_max_in_bytes { get; set; }
    public int non_heap_init_in_bytes { get; set; }
    public int non_heap_max_in_bytes { get; set; }
    public int direct_max_in_bytes { get; set; }
}

public class Module
{
    public string name { get; set; }
    public string version { get; set; }
    public string description { get; set; }
    public bool jvm { get; set; }
    public string classname { get; set; }
    public bool isolated { get; set; }
    public bool site { get; set; }
}

public class Network
{
    public string host { get; set; }
}

public class Os
{
    public int refresh_interval_in_millis { get; set; }
    public string name { get; set; }
    public string arch { get; set; }
    public string version { get; set; }
    public int available_processors { get; set; }
    public int allocated_processors { get; set; }
}

public class Path
{
    public string conf { get; set; }
    public string data { get; set; }
    public string logs { get; set; }
    public string home { get; set; }
}

public class Percolate
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public int queue_size { get; set; }
}

public class Ping
{
    public Unicast unicast { get; set; }
}

public class Process
{
    public int refresh_interval_in_millis { get; set; }
    public int id { get; set; }
    public bool mlockall { get; set; }
}

//public class Profiles
//{
//}

public class Refresh
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public string keep_alive { get; set; }
    public int queue_size { get; set; }
}


public class Search
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public int queue_size { get; set; }
}

public class Settings
{
    public string pidfile { get; set; }
    public Cluster cluster { get; set; }
    public Node node { get; set; }
    public Path path { get; set; }
    public Discovery discovery { get; set; }
    public string name { get; set; }
    public Client client { get; set; }
    public Http http { get; set; }
    public Transport transport { get; set; }
    public Config config { get; set; }
    public Network network { get; set; }
}

public class Bulk
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public int queue_size { get; set; }
}

public class Client
{
    public string type { get; set; }
}

public class Cluster
{
    public string name { get; set; }
}

public class Config
{
    public string ignore_system_properties { get; set; }
}

public class Discovery
{
    public Zen zen { get; set; }
}

public class FetchShardStarted
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public string keep_alive { get; set; }
    public int queue_size { get; set; }
}

public class FetchShardStore
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public string keep_alive { get; set; }
    public int queue_size { get; set; }
}

public class Flush
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public string keep_alive { get; set; }
    public int queue_size { get; set; }
}

public class ForceMerge
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public int queue_size { get; set; }
}

public class Generic
{
    public string type { get; set; }
    public string keep_alive { get; set; }
    public int queue_size { get; set; }
}

public class Get
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public int queue_size { get; set; }
}
public class Snapshot
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public string keep_alive { get; set; }
    public int queue_size { get; set; }
}

public class Suggest
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public int queue_size { get; set; }
}

public class Tcp
{
    public string port { get; set; }
}

public class ThreadPool
{
    public ForceMerge force_merge { get; set; }
    public Percolate percolate { get; set; }
    public FetchShardStarted fetch_shard_started { get; set; }
    public Listener listener { get; set; }
    public Index index { get; set; }
    public Refresh refresh { get; set; }
    public Suggest suggest { get; set; }
    public Generic generic { get; set; }
    public Warmer warmer { get; set; }
    public Search search { get; set; }
    public Flush flush { get; set; }
    public FetchShardStore fetch_shard_store { get; set; }
    public Management management { get; set; }
    public Get get { get; set; }
    public Bulk bulk { get; set; }
    public Snapshot snapshot { get; set; }
}

public class Transport
{
    public Tcp tcp { get; set; }
    public List<string> bound_address { get; set; }
    public string publish_address { get; set; }
    //public Profiles profiles { get; set; }
}

public class Unicast
{
    public List<string> hosts { get; set; }
}

public class Warmer
{
    public string type { get; set; }
    public int min { get; set; }
    public int max { get; set; }
    public string keep_alive { get; set; }
    public int queue_size { get; set; }
}

public class Zen
{
    public Ping ping { get; set; }
}


