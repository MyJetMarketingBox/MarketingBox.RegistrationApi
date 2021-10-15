﻿using System;
using System.Collections.Generic;

namespace MarketingBox.RegistrationApi.Pagination
{
    public static class Paginated
    {
        public static Paginated<TItem, TId> Empty<TItem, TId>(PaginationRequest<TId> request)
        {
            return new Paginated<TItem, TId>
            {
                Pagination = new Pagination<TId>
                {
                    Order = request.Order,
                    Cursor = request.Cursor
                },
                Items = ArraySegment<TItem>.Empty
            };
        }
    }

    public class Paginated<TItem, TId>
    {
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public Pagination<TId> Pagination { get; set; }
        public IReadOnlyCollection<TItem> Items { get; set; }
    }
}
