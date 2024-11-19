import os
from pyflink.table import TableEnvironment, EnvironmentSettings

def create():
    env_settings = EnvironmentSettings.in_streaming_mode()
    t_env = TableEnvironment.create(env_settings)
    current_directory = os.path.abspath(os.path.dirname(__file__))
    t_env.get_config().set("pipeline.jars", f"file:///{current_directory}/../target/flink-sql-connector-kafka-3.2.0-1.19.jar;file:///{current_directory}/../target/flink-json-1.17.2.jar")
    return t_env
