﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularProdject_API.Models;
using AngularProdject_API.Repositories.Interfaces;
using AngularProdject_API.ViewModels;

namespace AngularProdject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericRepo<Order> repo;
        private readonly IGenericRepo<OrderItem> itemRepo;
        public OrdersController(IUnitOfWork unitOfWork)
        {
           this.unitOfWork = unitOfWork;
           this.repo = this.unitOfWork.GetRepo<Order>();
            this.itemRepo = this.unitOfWork.GetRepo<OrderItem>();
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var data= await this.repo.GetAllAsync();
            return data.ToList();
        }
        [HttpGet("VM")]
        public async Task<ActionResult<IEnumerable<OrderViewModel>>> GetOrderVMs()
        {
           
            var data = await this.repo.GetAllAsync(x=> x.Include(o=> o.OrderItems).ThenInclude(oi => oi.Product)
                                                        .Include(o=> o.Customer));
            return data.Select(o=> new OrderViewModel
            {
                OrderID= o.OrderID,
                CustomerID = o.CustomerID,
                OrderDate =o.OrderDate,
                DeliveryDate=o.DeliveryDate,
                Status= o.Status,
                CustomerName= o.Customer.CustomerName,
                OrderValue = o.OrderItems.Sum(oi=> oi.Quantity*oi.Product.Price)
            })
            .ToList();
        }
        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await this.repo.GetAsync(o=> o.OrderID== id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }
        [HttpGet("{id}/OI")]
        public async Task<ActionResult<Order>> GetOrderWithOrderItems(int id)
        {
            var order = await this.repo.GetAsync(o => o.OrderID == id, x=> x.Include(o=> o.OrderItems).ThenInclude(oi=> oi.Product)
                                                                            .Include(o=> o.Customer));

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }
        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderID)
            {
                return BadRequest();
            }

            await this.repo.UpdateAsync(order);

            try
            {
                await this.unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
                    throw;
                
            }

            return NoContent();
        }
        [HttpPut("VM/{id}")]
        public async Task<IActionResult> PutOrderWithOrderItem(int id, Order order)
        {
            if (id != order.OrderID)
            {
                return BadRequest();
            }
            //var exisiting =await this.repo.GetAsync(o => o.OrderID == id, x=> x.Include(y=> y.OrderItems));
            //var items = exisiting.OrderItems.ToList();
            //for(var i=0; i<= items.Count; i++)
            //{
            //    await this.DeleteOrderItem(exisiting.OrderID, items[i].ProductID);
            //}
            //foreach(var x in order.OrderItems)
            //{
            //    exisiting.OrderItems.Add(x);
            //}
            await this.repo.UpdateAsync(order);

            try
            {
                await this.unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }

            return NoContent();
        }
        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            await this.repo.AddAsync(order);
            await this.unitOfWork.CompleteAsync();

            return order;
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await this.repo.GetAsync(o=> o.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            await this.repo.DeleteAsync(order);
            await this.unitOfWork.CompleteAsync();

            return NoContent();
        }
        
       

    }
}
