using QuanLi_CF.Domain;
using QuanLi_CF.Interface;
using QuanLi_CF.Repositories;
using QuanLi_CF.Services;
using QuanLi_CF.Strategies;
using QuanLi_CF.Events;
using QuanLi_CF.Exceptions; 
using System;
using System.Runtime.InteropServices;
namespace QuanLi_CF
{
    class Program
    {
        static void Main(string[] args)
        {
            var drinkRepo = new InMemoryRepository<DrinkProduct, string>(p => p.ProductID);
            var orderRepo = new InMemoryRepository<Order, string>(o => o.OrderNo);
            var inventoryService = new InventoryService(drinkRepo, 2);
            var orderService = new OrderService(inventoryService, orderRepo);
            orderService.PointsAccrued += (s, e) =>
              {
                  Console.WriteLine($"Khach hang {e.CustomerName} duoc cong {e.PointsAdded} diem. Tong diem hien tai: {e.TotalPoints}");
              };
            drinkRepo.Add(new DrinkProduct { Name = "Espresso", BasePrice = 25_000m, StockQty = 10 });
            drinkRepo.Add(new DrinkProduct { Name = "Cappuccino", BasePrice = 30_000m, StockQty = 10 });
            drinkRepo.Add(new DrinkProduct { Name = "Latte", BasePrice = 35000m, StockQty = 10 });
            var customer = new MemberCustomer { fullName = "Nguyen Van A", Point = 120, phone = "0123456789" };
            Order currentOrder = null;
            while (true)
            {
                Console.WriteLine("\n===== MENU QUAN CAFE =====");
                Console.WriteLine("1. XEM DANH SACH DO UONG");
                Console.WriteLine("2. TAO DON HANG MOI");
                Console.WriteLine("3. THEM SAN PHAM VAO DON HANG");
                Console.WriteLine("4. THANH TOAN");
                Console.WriteLine("0. THOAT");
                Console.Write("Chon chuc nang: ");
                var choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("\n--- DANH SACH DO UONG ---");
                            foreach (var drink in drinkRepo.GetAll())
                            {
                                Console.WriteLine($"{drink.ProductID.Substring(0, 4)} - {drink.Name} - {drink.BasePrice} VND - {drink.size} - Ton kho: {drink.StockQty}");
                            }
                            break;
                        case "2":
                            currentOrder = new Order(customer, new DefaultPriceRule(), new ComboDiscountPolicy());
                            Console.WriteLine($"Da tao don hang moi voi ma so: {currentOrder.OrderNo}");
                            break;
                        case "3":
                            if (currentOrder == null)
                            {
                                Console.WriteLine("Vui long tao don hang moi truoc khi them san pham.");
                                break;
                            }
                            Console.Write("Nhap ma san pham:");
                            var code = Console.ReadLine();
                            DrinkProduct product = null;
                            foreach (var p in drinkRepo.GetAll())
                            {
                                if (p.ProductID.StartsWith(code))
                                {
                                    product = p;
                                    break;
                                }
                            }
                            if (product == null) throw new OrderNotFoundException("Ma san pham khong hop le.");
                            Console.Write("Nhap so luong:");
                            int qty;
                            if (!int.TryParse(Console.ReadLine(), out qty) || qty <= 0) throw new FormatException("So luong khong hop le.");
                            var toppings = new List<ToppingItem>();
                            while (true)
                            {
                                Console.Write("Nhap ten topping hoac 'done' de ket thuc");
                                var tp = Console.ReadLine();
                                if (tp != null && tp.ToLower() == "done") break;
                                if (string.IsNullOrWhiteSpace(tp)) throw new InvalidToppingException("Ten topping khong hop le.");
                                Console.Write(" Moi ban nhap gia topping");
                                decimal toppingPrice;
                                if (!decimal.TryParse(Console.ReadLine(), out toppingPrice) || toppingPrice < 0) throw new FormatException("Gia topping khong hop le.");
                                toppings.Add(new ToppingItem { Name = tp, Price = toppingPrice });
                            }
                            var line = new OrderLine(product, qty, product.BasePrice);
                            foreach (var t in toppings)
                            {
                                line.Topping.Add(t);

                            }
                                currentOrder.AddLine(line);
                                currentOrder.RecalcTotals();
                                Console.WriteLine("Da them san pham vao don hang.");
                                break;
                           
                        case "4":
                            if (currentOrder == null)
                            {
                                Console.WriteLine("Vui long tao don hang moi truoc khi thanh toan.");
                                break;
                            }
                            Console.WriteLine("\n---HOA DON THANH TOAN---");
                            Console.WriteLine(currentOrder);
                            orderService.Submit(currentOrder);
                            orderService.Pay(currentOrder);
                            Console.WriteLine("Da thanh toan don hang.");
                            currentOrder = null;
                            break;
                        case "0":
                            Console.WriteLine("Cam on ban da su dung dich vu. Hen gap lai!");
                            return;
                        default:
                            Console.WriteLine("Chuc nang khong hop le. Vui long chon lai.");
                            break;
                    }
                }
                catch (OutofStockException ex)
                {
                    Console.WriteLine($"Loi kho : {ex.Message}");
                }
                catch (InvalidToppingException ex)
                {
                    Console.WriteLine($"Loi topping: {ex.Message}");
                }
                catch (OrderNotFoundException ex)
                {
                    Console.WriteLine($"Loi don hang: {ex.Message}");
                }
                catch (InvalidOrderStatusException ex)
                {
                    Console.WriteLine($"Loi trang thai don hang: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Co loi xay ra: {ex.Message}");
                }

            }
        }
    }
}