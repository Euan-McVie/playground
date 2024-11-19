import logging
import sys
import table_env
import market_depth_sink
import public_orders_source

from order_side_aggregator import order_agg
from pyflink.table.expressions import col

def calculate_market_depth():
    t_env = table_env.create()

    # Attach the source
    public_orders_source.attach(t_env)

    # Get the public orders
    public_orders = t_env.from_path("public_orders") \
        .group_by(col("tradingVenueId"), col("deliveryAreaId"), col("extContractId")) \
        .aggregate(order_agg.alias("orders")) \
        .select(col("*"))


    # Test the results
    public_orders.print_schema()
    #public_orders.execute().print()

    # Store the results
    #market_depth_sink.attach(t_env)
    #market_depth_sink.insert(public_orders).wait()

if __name__ == "__main__":
    logging.basicConfig(stream=sys.stdout, level=logging.INFO, format="%(message)s")
    calculate_market_depth()
