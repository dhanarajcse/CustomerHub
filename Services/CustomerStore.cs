using CustomerHub.Models;

namespace CustomerHub.Services;

public class CustomerStore
{
    private readonly List<Customer> _customers = new();
    private int _nextId = 1;

    public List<Customer> GetAll() => _customers;

    public Customer? GetById(int id)
    {
        return _customers.FirstOrDefault(c => c.Id == id);
    }

    public void Add(Customer customer)
    {
        customer.Id = _nextId++;
        _customers.Add(customer);
    }

    public void Update(Customer customer)
    {
        var existing = GetById(customer.Id);

        if (existing is null)
        {
            return;
        }

        existing.FullName = customer.FullName;
        existing.Email = customer.Email;
        existing.Phone = customer.Phone;
        existing.City = customer.City;
        existing.JoinedOn = customer.JoinedOn;
    }

    public void Delete(int id)
    {
        var customer = GetById(id);

        if (customer is not null)
        {
            _customers.Remove(customer);
        }
    }
}