using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MariosPizzaApp
{
    public partial class OrderForm : Form
    {
        // employee id
        string employeeId;

        // object lists for Pizza, Side and Drink
        List<Pizza> pizzas = new List<Pizza>();
        List<Side> sides = new List<Side>();
        List<Drink> drinks = new List<Drink>();
        List<Receipt> receipts = new List<Receipt>();

        // arrays for toppings and sizes
        string[] allToppings = new string[] { "Anchovies", "Black olives", "Peppers", "Jalapenos", "Mushroom", "Red onion", "Sweetcorn", "Pepperoni", "Pineapple", "Spicy beef", "Chicken", "Sausage", "Ham", "Tuna" };
        string[] allSizes = new string[] { "Small", "Medium", "Large" };

        // applied deal booleans
        bool deal1Active = false;
        bool deal2Active = false;

        public OrderForm(string employeeIdRef)
        {
            InitializeComponent();
            // pass employeeId from LoginForm to the constructor of OrderForm; Needed for receipt.
            employeeId = employeeIdRef;
        }

        private void OrderForm_Load(object sender, EventArgs e)
        {
            /**
             * ORDERFORM_LOAD
             ************************************************************
             * On form load, populate all combo boxes and checklist boxes with relevant information.
             * Whilst this is more work, it will allow us to edit all list entries
             * in one place. rather than going through each tabcontrol tab and changing
             * entries that way.
             **/

            // populate clbToppings checklist box with entries from allToppings array
            foreach (string topping in allToppings)
            {
                clbToppings.Items.Add(topping);
            }

            // populate cbSizes combobox with entries from allSizes array
            foreach (string size in allSizes)
            {
                cbSizes.Items.Add(size);
            }

            // remove receipt page on load - filled on completion
            tcMainForm.TabPages.Remove(tpReceipt);

            // set txtEmployeeID to employeeId to always show the staff member logged in
            txtEmployeeID.Text = employeeId; 
        }


        /**
         *  POPULATE METHODS
         *****************************************************
         * Methods to be called when the listboxes need to be
         * populated.
         * 
         * Methods should be called whenever a new object is 
         * added to any of the three lists.
         **/
        private void PopulatePizzaList()
        {
            /**
             * When method called, populate lsbPizzaItems with all entries
             * of the pizzas list, whilst clearing existing entries beforehand.
             **/

            // clear existing entries in lsbPizzaItems list box
            lsbPizzaItems.Items.Clear();

            // cycle through pizzas list, adding size and num of toppings to list box
            for (int i = 0; i < pizzas.Count; i++)
            {
                if (pizzas[i].size != null || !string.IsNullOrWhiteSpace(pizzas[i].size))
                {
                    lsbPizzaItems.Items.Add(string.Format("Size: {0} | # of toppings: {1} | Price: {2}", pizzas[i].size, pizzas[i].totalToppings.ToString(), $"{pizzas[i].price:C2}"));
                }
            }
        }

        private void PopulateSideList()
        {
            /**
             * When method called, populate lsbSideItems with all entries
             * of the sides list, whilst clearing existing entries beforehand.
             **/

            // clear all existing entries from lsbSidesItems
            lsbSideItems.Items.Clear();

            // cycle through sides list, adding side name and quantity to list box
            for (int i = 0; i < sides.Count; i++)
            {
                lsbSideItems.Items.Add(string.Format("{0} ({1}) | {2}", sides[i].name, sides[i].quantity.ToString(), $"{sides[i].price:C2}"));
            }
        }

        private void PopulateDrinkList()
        {
            /**
             * When method called, populate lsbDrinkItems with all entries
             * of the drinks list, whilst clearing existing entries beforehand.
             **/

            // clear all existing items in lsbDrinkItems
            lsbDrinkItems.Items.Clear();

            // cyle through drinks list, adding drink name and quantity to list box
            for (int i = 0; i < drinks.Count; i++)
            {
                lsbDrinkItems.Items.Add(string.Format("{0} ({1}) | {2}", drinks[i].name, drinks[i].quantity.ToString(), $"{drinks[i].price:C2}"));
            }
        }


        /**
         *  PIZZA METHODS
         *****************************************************
         * Methods to be called when creating a Pizza object
         * 
         * These are not calculation functions
         **/
        private void btnAddUpdatePizza_Click(object sender, EventArgs e)
        {
            if (btnAddUpdatePizza.Text == "Add Pizza")
            {
                // execute AddNewPizza method to create new pizza object from fields
                AddNewPizza();
            }
            else if (btnAddUpdatePizza.Text == "Update Pizza")
            {
                // execite UpdatePizza method to update the selected pizza object from the pizzas list
                UpdatePizza();
            }
        }

        private void AddNewPizza()
        {
            // create pizza object
            Pizza pizza = new Pizza();

            // initialise toppings list
            pizza.Toppings = new List<string>();

            // cycle through allSizes array, and collect the index
            // using the index of the selected item in the combobox
            for (int i = 0; i < allSizes.Length; i++)
            {
                if (cbSizes.SelectedIndex == i)
                {
                    pizza.size = allSizes[i];
                }
            }

            // cycle through allToppings array, and collect the index
            // using the index of the selected item in the combobox
            for (int i = 0; i < allToppings.Length; i++)
            {
                if (clbToppings.GetItemChecked(i) == true)
                {
                    pizza.Toppings.Add(allToppings[i]);
                }
            }

            // get number of toppings and saze in pizza.totalToppings
            pizza.totalToppings = pizza.Toppings.Count();

            // calculate base price of the pizza - easier for use in final calculation
            pizza.price = CalcBasePizzaPrice(pizza.size, pizza.totalToppings);

            if (pizza.size != null || !string.IsNullOrWhiteSpace(pizza.size))
            {
                // add pizza object to pizzas list
                pizzas.Add(pizza);
            }
            else
            {
                MessageBox.Show("Pizzas must have a selected size!");
            }

            // populate pizza list with new and existing entries
            PopulatePizzaList();
            UncheckToppingsCheckboxes();
        }

        private void UpdatePizza()
        {
            // set index of selected pizza from list box
            int n = lsbPizzaItems.SelectedIndex;

            if (n > -1)
            {
                // cycle through allSizes array
                for (int i = 0; i < allSizes.Length; i++)
                {
                    // cycle through combo box and check which is selected
                    if (cbSizes.SelectedIndex == i)
                    {
                        // set pizza size to selected size - from array
                        pizzas[n].size = allSizes[i];
                    }
                }

                // clear all toppings from this object's toppings list
                pizzas[n].Toppings.Clear();

                for (int i = 0; i < clbToppings.Items.Count; i++)
                {
                    // select all checked check boxes, and add their corresponding topping to the object's list
                    if (clbToppings.GetItemChecked(i) == true)
                    {
                        pizzas[n].Toppings.Add(allToppings[i]);
                    }

                    // unselect all check boxes, ensuring they are removed from toppings list
                    if (clbToppings.GetItemChecked(i) == false)
                    {
                        pizzas[n].Toppings.Remove(allToppings[i]);
                    }
                }

                // calc total toppings
                pizzas[n].totalToppings = pizzas[n].Toppings.Count;

                // calc base price fusing size and total toppings
                pizzas[n].price = CalcBasePizzaPrice(pizzas[n].size, pizzas[n].totalToppings);

                // set button text back to "Add Pizza"
                btnAddUpdatePizza.Text = "Add Pizza";

                // re-populate pizza list box, ensuring update takes affect
                PopulatePizzaList();

                UncheckToppingsCheckboxes();
            }
        }

        private void UncheckToppingsCheckboxes()
        {
            for (int i = 0; i < clbToppings.Items.Count; i++)
            {
                clbToppings.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void btnDeletePizza_Click(object sender, EventArgs e)
        {
            int n = lsbPizzaItems.SelectedIndex;

            pizzas.RemoveAt(n);

            PopulatePizzaList();

            btnAddUpdatePizza.Text = "Add Pizza";
        }

        private void lsbPizzaItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAddUpdatePizza.Text = "Update Pizza";

            int n = lsbPizzaItems.SelectedIndex;

            if (n > -1)
            {
                // send to pizza tab
                tcMainForm.SelectedIndex = 0;

                // uncheck all clbToppings checkboxes 
                for (int i = 0; i < clbToppings.Items.Count; i++)
                {
                    clbToppings.SetItemChecked(i, false);
                }

                // set cbSizes to appropriate size
                cbSizes.SelectedItem = pizzas[n].size;

                // if pizza has topping, set appropriate checkbox to true
                for (int i = 0; i < allToppings.Length; i++)
                {
                    if (pizzas[n].Toppings.Contains(allToppings[i]) == true)
                    {
                        clbToppings.SetItemChecked(i, true);
                    }
                }
            }
        }


        /**
         *  SIDE METHODS
         *****************************************************
         * Methods to be called when creating a Side object
         * 
         * These are not calculation functions
         **/
        private void btnAddSides_Click(object sender, EventArgs e)
        {
            // clear current sides list - used for updating sides
            sides.Clear();

            // check if numeric up down boxes are greater than 0, and if they are
            // create an object for the specific side. 
            if (nudSidesGB.Value > 0)
            {
                string name = "Garlic bread";
                decimal price = 1.70m;
                int quantity = Convert.ToInt32(nudSidesGB.Value);

                // call the AddSIdes method which uses the name, price and quantity of the
                // side to create a Side object and add it to the sides list
                AddSide(name, price, quantity);
            }

            if (nudSidesGBC.Value > 0)
            {
                string name = "Garlic bread with cheese";
                decimal price = 2.20m;
                int quantity = Convert.ToInt32(nudSidesGBC.Value);

                AddSide(name, price, quantity);
            }

            if (nudSidesSCW.Value > 0)
            {
                string name = "Spicy chicken wings";
                decimal price = 3.50m;
                int quantity = Convert.ToInt32(nudSidesSCW.Value);

                AddSide(name, price, quantity);
            }

            if (nudSidesFFReg.Value > 0)
            {
                string name = "French fries [R]";
                decimal price = 1.00m;
                int quantity = Convert.ToInt32(nudSidesFFReg.Value);

                AddSide(name, price, quantity);
            }

            if (nudSidesFFLarge.Value > 0)
            {
                string name = "French fries [L]";
                decimal price = 1.30m;
                int quantity = Convert.ToInt32(nudSidesFFLarge.Value);

                AddSide(name, price, quantity);
            }

            if (nudSidesC.Value > 0)
            {
                string name = "Coleslaw";
                decimal price = 0.70m;
                int quantity = Convert.ToInt32(nudSidesC.Value);

                AddSide(name, price, quantity);
            }

            btnAddSides.Text = "Update Side(s)";
        }

        private void AddSide(string name, decimal price, int quantity)
        {
            /**
             * Called when creating new Side object, and uses passed variables
             * to create a new Side object, which is added the the sides list
             * and the lsbSideItems list is re-populated.
             **/
            Side side = new Side();
            side.name = name;
            side.quantity = quantity;

            if (side.name == "Spicy chicken wings")
            {
                side.price += (decimal)(side.quantity / 10 * 6.00);

                if (side.quantity % 10 == 5)
                {
                    side.price += 3.50m;
                }
            }
            else
            {
                side.price = price * quantity;
            }

            // add side to list
            sides.Add(side);

            // populate lsbSideItems using items in sides list
            PopulateSideList();
        }


        /**
         *  DRINK METHODS
         *****************************************************
         * Methods to be called when creating a Drink object
         * 
         * These are not calculation functions
         **/
        private void btnAddDrinks_Click(object sender, EventArgs e)
        {
            // clear current items in drinks list - used in updating
            drinks.Clear();

            // check if numeric up down boxes are greater than 0, and if they are
            // create an object for the specific drink. 
            if (nudDrinksCoke.Value > 0)
            {
                string name = "Coke";
                int quantity = Convert.ToInt32(nudDrinksCoke.Value);

                // call AddDrinks method which uses the drink's name and quantity
                // to create a Drink object and populate the drinks list
                AddDrinks(name, quantity);
            }

            if (nudDrinksPepsi.Value > 0)
            {
                string name = "Pepsi";
                int quantity = Convert.ToInt32(nudDrinksPepsi.Value);

                AddDrinks(name, quantity);
            }

            if (nudDrinksDCoke.Value > 0)
            {
                string name = "Diet Coke";
                int quantity = Convert.ToInt32(nudDrinksDCoke.Value);

                AddDrinks(name, quantity);
            }

            if (nudDrinks7Up.Value > 0)
            {
                string name = "7-Up";
                int quantity = Convert.ToInt32(nudDrinks7Up.Value);

                AddDrinks(name, quantity);
            }

            if (nudDrinksFanta.Value > 0)
            {
                string name = "Fanta";
                int quantity = Convert.ToInt32(nudDrinksFanta.Value);

                AddDrinks(name, quantity);
            }

            if (nudDrinksTango.Value > 0)
            {
                string name = "Tango";
                int quantity = Convert.ToInt32(nudDrinksTango.Value);

                AddDrinks(name, quantity);
            }

            btnAddDrinks.Text = "Update Drink(s)";
        }

        private void AddDrinks(string name, int quantity)
        {
            /**
             * Called when creating new Drink objects. Uses passed variables when
             * creating a Drink object, and then adds the Drink to the drinks list
             * and intitialises the population of the lsbDrinkItems list box.
             **/
            Drink drink = new Drink();
            drink.name = name;
            drink.quantity = quantity;
            drink.price = 0.70m * quantity;

            // add drink to drinks list
            drinks.Add(drink);

            // initialise population of lsbDrinkItems list box
            PopulateDrinkList();
        }


        /**
         *  NAVIGATION METHODS
         *****************************************************
         * Methods to be called when navigating tabs in the tab control
         **/
        private void NextTab()
        {
            // get index of current tab, then adding 1 to it
            int i = tcMainForm.SelectedIndex + 1;

            // set selected tab to 1 greater than current tab
            tcMainForm.SelectedIndex = i;
        }

        private void PrevTab()
        {
            // get index of current tab, then remove one from it
            int i = tcMainForm.SelectedIndex - 1;

            // set selected tab to 1 less than current tab
            tcMainForm.SelectedIndex = i;
        }

        private void btnToDrinks_Click(object sender, EventArgs e)
        {
            NextTab();
        }

        private void btnToSides_Click(object sender, EventArgs e)
        {
            NextTab();
        }

        private void btnToOverview_Click(object sender, EventArgs e)
        {
            NextTab();
        }

        private void btnBackToPizza_Click(object sender, EventArgs e)
        {
            PrevTab();
        }

        private void btnBackToSides_Click(object sender, EventArgs e)
        {
            PrevTab();
        }

        private void btnBackToDrinks_Click(object sender, EventArgs e)
        {
            PrevTab();
        }

        private void tcMainForm_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Calculate base price of order when order overview tabpage is selected
            if (tcMainForm.SelectedIndex == 3)
            {
                decimal orderBasePrice = 0;

                // cycle through all Pizza objects, getting price
                foreach (Pizza pizza in pizzas)
                {
                    orderBasePrice += pizza.price;
                }

                // cycle through all Side objects, getting price
                foreach (Side side in sides)
                {
                    orderBasePrice += side.price;
                }

                // cycle through all Drink objects, getting price
                foreach (Drink drink in drinks)
                {
                    orderBasePrice += drink.price;
                }

                // display price in txtOrderBasePrice
                txtOrderBasePrice.Text = $"{orderBasePrice:C2}";

                // populate deals from items in lists
                PopulateDeals();

                // display final price in txtOrderFinalPrice, where value is returned by CalcFinalPrice
                txtOrderFinalPrice.Text = $"{CalcFinalPrice():C2}";
            }

            // if user clicks the receipt tab, repopulate fresh
            if (tcMainForm.SelectedIndex == 4)
            {
                if (!string.IsNullOrWhiteSpace(txtOrderName.Text) && !string.IsNullOrWhiteSpace(txtOrderAddress.Text) && !string.IsNullOrWhiteSpace(txtOrderPostCode.Text))
                {
                    CreateReceipt();
                    PopulateReceipt();
                }
            }
        }


        /**
         *  OVERVIEW METHODS
         *****************************************************
         * Methods used when the tab control tab page is on index
         * 3 - overview tab. This tab needs to be populated with 
         * the order on load
         **/
        private void PopulateDeals()
        {
            // clear all deals to prevent no duplicates
            clbOrderDeals.Items.Clear();

            int numLarge = 0;

            // cycle through pizzas, checking for deal prerequisites
            foreach (Pizza pizza in pizzas)
            {
                if (pizza.size == "Medium" && pizza.totalToppings == 4)
                {
                    if (drinks.Count > 0)
                    {
                        clbOrderDeals.Items.Add("1 Medium pizza, 4 toppings, 1 drink");
                    }
                }

                if (pizza.size == "Large" && pizza.totalToppings == 4)
                {
                    numLarge++;
                }
            }

            if (numLarge == 2)
            {
                clbOrderDeals.Items.Add("2 Large pizzas, 4 toppings");
            }
        }

        private void clbOrderDeals_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            decimal orderTotal = 0;

            // get selected deal
            string deal = clbOrderDeals.SelectedItem.ToString();

            // if checked is true, set corresponding deal to true
            if (e.NewValue == CheckState.Checked)
            {
                if (deal == "1 Medium pizza, 4 toppings, 1 drink")
                {
                    deal1Active = true;
                }

                if (deal == "2 Large pizzas, 4 toppings")
                {
                    deal2Active = true;
                }
            }
            // if checked is false, set corresponding deal back to false
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (deal == "1 Medium pizza, 4 toppings, 1 drink")
                {
                    deal1Active = false;
                }

                if (deal == "2 Large pizzas, 4 toppings")
                {
                    deal2Active = false;
                }
            }

            // get final price using deals above
            orderTotal = CalcFinalPrice();

            // display price in txtOrderFinalPrice
            txtOrderFinalPrice.Text = $"{orderTotal:C2}";
        }


        /**
         *  RECEIPT METHODS
         *****************************************************
         * Constructing and populating the final Receipt Object
         **/
        private void CreateReceipt()
        {
            // clear all receipts so there can only be 1 at a time
            receipts.Clear();

            // new receipt object
            Receipt receipt = new Receipt();

            // populate receipt from text boxes on overview tab page
            receipt.name = txtOrderName.Text;
            receipt.addressLine1 = txtOrderAddress.Text;
            receipt.postcode = txtOrderPostCode.Text;

            // intialise all lists
            receipt.OrderPizzas = new List<Pizza>();
            receipt.OrderSides = new List<Side>();
            receipt.OrderDrinks = new List<Drink>();
            receipt.OrderDeals = new List<string>();

            // add each Pizza to receipt
            foreach (Pizza pizza in pizzas)
            {
                receipt.OrderPizzas.Add(pizza);
            }

            // add each Side to receipt
            foreach (Side side in sides)
            {
                receipt.OrderSides.Add(side);
            }

            // add each Drink to receipt
            foreach (Drink drink in drinks)
            {
                receipt.OrderDrinks.Add(drink);
            }

            // add deals to receipt
            if (deal1Active == true)
            {
                receipt.OrderDeals.Add("1 Medium pizza, 4 toppings, 1 drink");
            }

            if (deal2Active == true)
            {
                receipt.OrderDeals.Add("2 Large pizzas, 4 toppings");
            }

            // calc price, delivery cost and total 
            receipt.orderCost = CalcFinalPrice();
            receipt.deliveryCost = (5 / receipt.orderCost) * 100;
            receipt.receiptTotal = receipt.orderCost + receipt.deliveryCost;

            // add receipt to receipt list
            receipts.Add(receipt);
        }

        private void PopulateReceipt()
        {
            // clear list box of final order
            lsbReceiptOrder.Items.Clear();
            lsbReceiptDeals.Items.Clear();

            // always use index of 0 as there can only be 1 item in the receipts list
            txtReceiptName.Text = receipts[0].name;
            txtReceiptAddress.Text = receipts[0].addressLine1;
            txtReceiptPostCode.Text = receipts[0].postcode;

            // heading for pizzas
            lsbReceiptOrder.Items.Add("[ DRINKS ]===============");

            // display in lsbReceiptOrder list box in appropriate format
            foreach (Pizza pizza in receipts[0].OrderPizzas)
            {
                lsbReceiptOrder.Items.Add(string.Format("{0} ({1}) [{2}]", pizza.size, pizza.totalToppings, $"{pizza.price:C2}"));

                for (int i = 0; i < pizza.Toppings.Count; i++)
                {
                    lsbReceiptOrder.Items.Add(string.Format(" - {0}", pizza.Toppings[i]));
                }
            }

            // divider for sides
            lsbReceiptOrder.Items.Add("");
            lsbReceiptOrder.Items.Add("[ SIDES ]================");

            // display in lsbReceiptOrder list box in appropriate format
            foreach (Side side in receipts[0].OrderSides)
            {
                lsbReceiptOrder.Items.Add(string.Format("{0} ({1}) [{2}]", side.name, side.quantity, $"{side.price:C2}"));
            }

            // divider for drinks
            lsbReceiptOrder.Items.Add("");
            lsbReceiptOrder.Items.Add("[ DRINKS ]===============");

            // display in lsbReceiptOrder list box in appropriate format
            foreach (Drink drink in receipts[0].OrderDrinks)
            {
                lsbReceiptOrder.Items.Add(string.Format("{0} ({1}) [{2}]", drink.name, drink.quantity, $"{drink.price:C2}"));
            }

            // display in lsbReceiptDeals list box
            foreach (string deal in receipts[0].OrderDeals)
            {
                lsbReceiptDeals.Items.Add(deal);
            }

            // display prices in text boxes
            txtReceiptOrderPrice.Text = $"{receipts[0].orderCost:C2}";
            txtReceiptDelivery.Text = $"{receipts[0].deliveryCost:C2}";
            txtReceiptFinalPrice.Text = $"{receipts[0].receiptTotal:C2}";

            // display employee id 
            txtReceiptEmployeeId.Text = employeeId;
        }

        private void btnFinishOrder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtOrderName.Text) && !string.IsNullOrWhiteSpace(txtOrderAddress.Text) && !string.IsNullOrWhiteSpace(txtOrderPostCode.Text))
            {
                // move from overview -> receipt
                tcMainForm.TabPages.Add(tpReceipt);
                CreateReceipt();
                PopulateReceipt();
                NextTab();
            }
            else
            {
                MessageBox.Show("Customer name, address and postcode fields must be filled!");
            }
        }


        /**
         *  FINAL METHODS
         *****************************************************
         * Methods used when order is completed
         **/
        private void btnNextOrder_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you would like to create a new order?", "Ordering System", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // close current form, create new OrderForm (ensuring to pass employeeId), then show new form.
                this.Close();

                OrderForm newOrder = new OrderForm(employeeId);
                newOrder.Show();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you would like to close this application?", "Ordering System", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // close environmet
                Environment.Exit(0);
            }
        }


        /**
         *  CALCULATION FUNCTIONS
         *****************************************************
         * All methods used to perform and/or return a calulated value.
         * 
         * These are called when calculating prices for the objects
         **/
        private decimal CalcBasePizzaPrice(string size, int toppings)
        {
            /**
             * Using the size and number of toppings passed to the method,
             * calculate the base price depending on the size of the pizza
             * chosen, and number of toppings where each topping is 80p
             **/
            decimal price = 0;

            if (size == "Small")
            {
                return price = (decimal)(3.50 + (0.80 * toppings));
            }
            else if (size == "Medium")
            {
                return price = (decimal)(5.00 + (0.80 * toppings));
            }
            else if (size == "Large")
            {
                return price = (decimal)(7.00 + (0.80 * toppings));
            }
            else
            {
                // return 0 if something went wrong
                return 0;
            }
        }

        private decimal CalcFinalPrice()
        {
            /**
             * Cycle through all objects in the order, adding price to orderTotal
             * which is then returned once the calculation is complete.
             **/
            decimal orderTotal = 0;

            foreach (Pizza pizza in pizzas)
            {
                orderTotal += pizza.price;
            }

            foreach (Side side in sides)
            {
                orderTotal += side.price;
            }

            foreach (Drink drink in drinks)
            {
                orderTotal += drink.price;
            }

            // deals merely minus the amount which is saved, rather than recalculating
            if (deal1Active == true)
            {
                orderTotal -= 1.00m;
            }

            if (deal2Active == true)
            {
                orderTotal -= 4.41m;
            }

            // return orderTotal
            return orderTotal;
        }
    }

    class Pizza
    {
        /**
         * PIZZA OBJECT
         ************************************************************
         * For each pizza added to the order, an object is craeted
         * which holds all information associated with this specific
         * pizza. These are then added to a list when the add button 
         * is pressed.
         **/

        public string size;
        public List<string> Toppings { get; set; }
        public int totalToppings;
        public decimal price;
    }

    class Side
    {
        /**
         * SIDE OBJECT
         ************************************************************
         * Similarly to the pizza object, when a side is selected, we
         * collect all information about the side selected. When selected
         * the employee will press the add button and the side will be
         * added to the sides list.
         **/

        public string name;
        public int quantity;
        public decimal price;
    }
    
    class Drink
    {
        /**
         * DRINK OBJECT
         ************************************************************
         * Similarly to the pizza and drink object, when a side is selected, 
         * we collect all information about the side selected. When selected
         * the employee will press the add button and the drink(s) will be
         * added to the drinks list.
         **/

        public string name;
        public int quantity;
        public decimal price;
    }

    class Receipt
    {
        /**
         * RECEIPT OBJECT
         ************************************************************
         * The Order object will be where all of the order will be collected
         * at the final stage. This is mainly here so we can easily add a future
         * improvement; orders taking to a MySQL database.
         **/

        // customer details
        public string name;
        public string addressLine1;
        public string postcode;

        // order details
        public List<Pizza> OrderPizzas { get; set; }
        public List<Side> OrderSides { get; set; }
        public List<Drink> OrderDrinks { get; set; }
        public List<string> OrderDeals { get; set; }

        // order costs - total + delivery cost
        public decimal orderCost;
        public decimal deliveryCost;
        public decimal receiptTotal;
    }
}