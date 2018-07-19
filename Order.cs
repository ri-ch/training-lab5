namespace StateMachines
{
    public class Order
    {
        public Order(int quantity, string item)
        {
            Quantity = quantity;
            Item = item;
        }

        public int Quantity { get; set; }
        public string Item { get; set; }
    }
}