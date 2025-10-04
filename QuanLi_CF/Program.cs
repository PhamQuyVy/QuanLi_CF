using QuanLi_CF.Domain;
using QuanLi_CF.Interface;
using QuanLi_CF.Repositories;
using QuanLi_CF.Services;
using QuanLi_CF.Strategies;
using QuanLi_CF.Events;
using QuanLi_CF.Exceptions;
using System;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var drinkRepo = new InMemoryRepository<DrinkProduct, string>(p => p.ProductID);
        var orderRepo = new InMemoryRepository<Order, string>(o => o.OrderNo);
        var toppingRepo = new InMemoryRepository<ToppingItem, string>(t => t.Name);
        var customerRepo = new InMemoryRepository<Customer, string>(c => c.CustomerID);

        var inventoryService = new InventoryService(drinkRepo, 2);
        var orderService = new OrderService(inventoryService, orderRepo);

        orderService.PointsAccrued += (s, e) =>
        {
            Console.WriteLine($"Khach hang {e.CustomerName} duoc cong {e.PointsAdded} diem. Tong diem hien tai: {e.TotalPoints}");
        };

        
        drinkRepo.Add(new DrinkProduct { Name = "Espresso", BasePrice = 25000m, StockQty = 10 });
        drinkRepo.Add(new DrinkProduct { Name = "Cappuccino", BasePrice = 30000m, StockQty = 10 });
        drinkRepo.Add(new DrinkProduct { Name = "Latte", BasePrice = 35000m, StockQty = 10 });

      
        toppingRepo.Add(new ToppingItem { Name = "Whipped Cream", Price = 5000m });
        toppingRepo.Add(new ToppingItem { Name = "Caramel", Price = 7000m });
        toppingRepo.Add(new ToppingItem { Name = "Chocolate", Price = 6000m });
        toppingRepo.Add(new ToppingItem { Name = "Ice", Price = 3000m });
        toppingRepo.Add(new ToppingItem { Name = "Boba", Price = 10000m });

    
        var member1 = new MemberCustomer { MemberCode = "M001", fullName = "Nguyen Van A", Phone = "0123456789", Tier = MemberTier.Bronze, Point = 50 };
        var member2 = new MemberCustomer { MemberCode = "M002", fullName = "Tran Thi B", Phone = "0987654321", Tier = MemberTier.Silver, Point = 120 };
        var member3 = new MemberCustomer { MemberCode = "M003", fullName = "Le Van C", Phone = "0911222333", Tier = MemberTier.Gold, Point = 250 };
        var member4 = new MemberCustomer { MemberCode = "M004", fullName = "Pham Thi D", Phone = "0909888777", Tier = MemberTier.Platinum, Point = 500 };

        customerRepo.Add(member1);
        customerRepo.Add(member2);
        customerRepo.Add(member3);
        customerRepo.Add(member4);

        Order currentOrder = null;
        Customer currentCustomer = null;

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
                        Console.WriteLine("Chon loai khach hang:");
                        Console.WriteLine("1. Khach hang da co thanh vien");
                        Console.WriteLine("2. Khach hang chua co thanh vien");
                        Console.Write("Lua chon: ");
                        var customerType = Console.ReadLine();

                        if (customerType == "1")
                        {
                            Console.WriteLine("Danh sach khach hang thanh vien:");
                            foreach (var c in customerRepo.GetAll().OfType<MemberCustomer>())
                            {
                                Console.WriteLine($"{c.MemberCode} - {c.fullName} - {c.Tier} - Diem: {c.Point}");
                            }
                            Console.Write("Nhap MemberCode: ");
                            string memberCode = Console.ReadLine();

                            var member = customerRepo.GetAll().OfType<MemberCustomer>()
                                .FirstOrDefault(m => m.MemberCode.Equals(memberCode, StringComparison.OrdinalIgnoreCase));
                            if (member == null)
                            {
                                Console.WriteLine("Khach hang khong ton tai. Tao WalkInCustomer mac dinh.");
                                currentCustomer = new WalkInCustomer { fullName = "Khach hang khong xac dinh", Phone = "" };
                            }
                            else currentCustomer = member;
                        }
                        else if (customerType == "2")
                        {
                            Console.Write("Nhap ten khach hang: ");
                            string name = Console.ReadLine();
                            Console.Write("Nhap so dien thoai: ");
                            string phone = Console.ReadLine();
                            currentCustomer = new WalkInCustomer { fullName = name, Phone = phone };
                        }
                        else
                        {
                            Console.WriteLine("Lua chon khong hop le. Tao WalkInCustomer mac dinh.");
                            currentCustomer = new WalkInCustomer { fullName = "Khach hang khong xac dinh", Phone = "" };
                        }

                        currentOrder = new Order(currentCustomer, new DefaultPriceRule(), new ComboDiscountPolicy());
                        Console.WriteLine($"Da tao don hang moi voi ma so: {currentOrder.OrderNo}");
                        break;

                    case "3":
                        if (currentOrder == null)
                        {
                            Console.WriteLine("Vui long tao don hang moi truoc khi them san pham.");
                            break;
                        }

                        Console.Write("Nhap ma san pham: ");
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

                        Console.Write("Nhap so luong: ");
                        int qty;
                        if (!int.TryParse(Console.ReadLine(), out qty) || qty <= 0) throw new FormatException("So luong khong hop le.");

                     
                        var selectedToppings = new List<ToppingItem>();
                        while (true)
                        {
                            Console.WriteLine("Chon topping:");
                            foreach (var t in toppingRepo.GetAll())
                            {
                                Console.WriteLine($"- {t.Name} ({t.Price} VND)");
                            }
                            Console.Write("Nhap ten topping hoac 'done' de ket thuc: ");
                            var tpName = Console.ReadLine();
                            if (tpName.ToLower() == "done") break;

                            var topping = toppingRepo.GetAll().FirstOrDefault(t => t.Name.Equals(tpName, StringComparison.OrdinalIgnoreCase));
                            if (topping == null) throw new InvalidToppingException("Ten topping khong hop le.");

                            selectedToppings.Add(topping);
                        }

                      
                        var line = new OrderLine(product, qty, product.BasePrice);
                        foreach (var t in selectedToppings)
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

                        Console.WriteLine("\n--- HOA DON THANH TOAN ---");
                        Console.WriteLine(currentOrder);

                       
                        orderService.Submit(currentOrder);
                        orderService.Pay(currentOrder);

                        Console.WriteLine("Da thanh toan don hang.");

                      
                        if (currentCustomer is WalkInCustomer)
                        {
                            Console.Write("Khach hang co muon dang ky thanh vien? (y/n): ");
                            var reg = Console.ReadLine();
                            if (reg.ToLower() == "y")
                            {
                                Console.Write("Nhap MemberCode: ");
                                string memberCode = Console.ReadLine();
                                Console.Write("Nhap ten: ");
                                string memberName = Console.ReadLine();
                                Console.Write("Nhap so dien thoai: ");
                                string memberPhone = Console.ReadLine();

                                var newMember = new MemberCustomer
                                {
                                    MemberCode = memberCode,
                                    fullName = memberName,
                                    Phone = memberPhone,
                                    Point = (int)Math.Floor(currentOrder.Total / 50000m),
                                    Tier = MemberTier.Standard
                                };
                                customerRepo.Add(newMember);
                                Console.WriteLine($"Khach hang {newMember.fullName} da duoc cong {newMember.Point} diem.");
                            }
                        }

                        currentOrder = null;
                        currentCustomer = null;
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
                Console.WriteLine($"Loi kho: {ex.Message}");
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