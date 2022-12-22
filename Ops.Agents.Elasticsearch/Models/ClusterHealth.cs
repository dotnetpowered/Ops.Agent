namespace Ops.Agents.Elasticsearch.Models;

public class EsClusterHealth
{
    public string cluster_name { get; set; }
    public string status { get; set; }
    public bool timed_out { get; set; }
    public int number_of_nodes { get; set; }
    public int number_of_data_nodes { get; set; }
    public int active_primary_shards { get; set; }
    public int active_shards { get; set; }
    public int relocating_shards { get; set; }
    public int initializing_shards { get; set; }
    public int unassigned_shards { get; set; }
    public int delayed_unassigned_shards { get; set; }
    public int number_of_pending_tasks { get; set; }
    public int number_of_in_flight_fetch { get; set; }
    public int task_max_waiting_in_queue_millis { get; set; }
    public double active_shards_percent_as_number { get; set; }
}
