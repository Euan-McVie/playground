import logging
import os
import sys

from pyflink.table import TableEnvironment, EnvironmentSettings

def market_depth():
    env_settings = EnvironmentSettings.in_streaming_mode()
    t_env = TableEnvironment.create(env_settings)
    current_directory = os.path.abspath(os.path.dirname(__file__))
    t_env.get_config().set("pipeline.jars", f"file:///{current_directory}/target/flink-sql-connector-kafka-3.2.0-1.19.jar;file:///{current_directory}/target/flink-json-1.17.2.jar")
    public_orders = t_env.from_elements([('ABC', 2334, 56200), ('DEF', 6666, 18400)], ['order_id', 'price', 'volume'])

    sink_ddl = """
    CREATE TABLE market_depth(
        order_id VARCHAR,
        price BIGINT,
        volume BIGINT
    ) WITH (
        'connector' = 'kafka',
        'topic' = 'market-depth',
        'properties.bootstrap.servers' = 'mynamespace.servicebus.windows.net:9093',
        'properties.group.id' = 'market_depth_calculator',
        'properties.client.id' = 'market_depth_calculator',
        'properties.security.protocol' = 'SASL_SSL',
        'properties.sasl.mechanism' = 'PLAIN',
        'properties.sasl.jaas.config' = 'org.apache.kafka.common.security.plain.PlainLoginModule required username="$ConnectionString" password="Endpoint=sb://mynamespace.servicebus.windows.net/;SharedAccessKeyName=XXXXXX;SharedAccessKey=XXXXXX";',
        'scan.startup.mode' = 'latest-offset',
        'format' = 'json'
    )
    """

    print_ddl = """
    CREATE TABLE market_depth(
        order_id VARCHAR,
        price BIGINT,
        volume BIGINT
    ) WITH (
        'connector' = 'print'
    )
    """
    t_env.execute_sql(sink_ddl)

    public_orders.execute_insert("market_depth").wait()

if __name__ == '__main__':
    logging.basicConfig(stream=sys.stdout, level=logging.INFO, format="%(message)s")
    market_depth()
