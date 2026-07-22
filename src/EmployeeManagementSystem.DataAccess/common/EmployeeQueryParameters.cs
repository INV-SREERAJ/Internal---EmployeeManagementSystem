using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystem.DataAccess.common;

public class EmployeeQueryParameters
{
    private const int MaxPageSize = 100;

    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    // Future Features

    public string? Search { get; set; }

    public string? Role { get; set; }

    public bool? IsActive { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}
