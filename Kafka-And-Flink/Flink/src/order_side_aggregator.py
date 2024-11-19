from pyflink.common import Row
from pyflink.table import DataTypes, MapView
from pyflink.table.udf import udaf, AggregateFunction

side_depth_data_type = DataTypes.MAP_VIEW(DataTypes.STRING(), DataTypes.STRING())

contract_depth_data_type = DataTypes.ROW([
            DataTypes.FIELD("buyOrders", side_depth_data_type),
            DataTypes.FIELD("sellOrders", side_depth_data_type)])

class OrderAggregator(AggregateFunction):

    def create_accumulator(self):
        return Row(MapView(), MapView())

    def get_value(self, accumulator):
        return accumulator.orders

    def accumulate(self, accumulator, row):
        accumulator.orders.add(row[0])

    def get_accumulator_type(self):
        return contract_depth_data_type

    def get_result_type(self):
        return contract_depth_data_type

order_agg = udaf(OrderAggregator())
