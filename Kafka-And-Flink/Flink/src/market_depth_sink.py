import env
from pyflink.table import TableEnvironment, Table

topic = "market-depth"
access_key = env.event_hub_market_depth_key

event_hub_connection_string = f"Endpoint=sb://{env.event_hub_host}/;SharedAccessKeyName={env.client_id};SharedAccessKey={access_key};EntityPath={topic}"

def attach(t_env: TableEnvironment):
    sink_ddl = f"""
    CREATE TABLE market_depth(
        tradingVenueId VARCHAR,
        extContractId VARCHAR,
        extOrderId VARCHAR,
        revision BIGINT,
        side VARCHAR,
        deliveryAreaId VARCHAR,
        price BIGINT,
        quantity BIGINT,
        state VARCHAR,
        createdAt TIMESTAMP_LTZ(9),
        orderBookUpdatedAt TIMESTAMP_LTZ(9),
        adapterReceivedAt TIMESTAMP_LTZ(9)
    ) WITH (
        'connector' = 'kafka',
        'topic' = '{topic}',
        'properties.bootstrap.servers' = '{env.event_hub_host}:9093',
        'properties.client.id' = '{env.client_id}',
        'properties.security.protocol' = 'SASL_SSL',
        'properties.sasl.mechanism' = 'PLAIN',
        'properties.sasl.jaas.config' = 'org.apache.flink.kafka.shaded.org.apache.kafka.common.security.plain.PlainLoginModule required username="$ConnectionString" password="{event_hub_connection_string}";',
        'format' = 'json',
        'json.timestamp-format.standard' = 'ISO-8601'
    )
    """

    t_env.execute_sql(sink_ddl)

def insert(table: Table):
    return table.execute_insert("market_depth")
