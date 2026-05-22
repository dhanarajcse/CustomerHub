using CustomerHub.Models;
using CustomerHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerHub.Controllers;

public class CustomersController : Controller
{
    private readonly CustomerStore _store;

    public CustomersController(CustomerStore store)
    {
        _store = store;
    }

    public IActionResult Index()
    {
        return View(_store.GetAll());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        _store.Add(customer);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var customer = _store.GetById(id);

        if (customer is null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    public IActionResult Edit(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        _store.Update(customer);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        _store.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}