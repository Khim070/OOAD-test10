using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace productlib
{
    public class ProductService
    {
        private static readonly List<ProductCreateReq> reqs =
        [
            new()
            {
                Code = "PRD001",
                Name = "Coca",
                Category = "Food"
            },
            new()
            {
                Code = "PRD002",
                Name = "Honda",
                Category = "Vehicle"
            },
            new()
            {
                Code = "PRD003",
                Name = "T-short",
                Category = "Cloth"
            },
        ];

        public static List<ProductCreateReq> InitRequest => reqs;

        private readonly ProductRepo _repo;
        
        public ProductService(ProductRepo repo)
        {
            _repo = repo;
        }

        public string? Create(ProductCreateReq req)
        {
            req.Code = req.Code.Trim();
            if (string.IsNullOrEmpty(req.Code)) return null;
            var found = _repo.GetQueryable().FirstOrDefault(x => string.Equals(
                                x.Code, req.Code, StringComparison.OrdinalIgnoreCase));
            if (found != null) return null;

            Product prd = req.ToEntity();
            _repo.Create(prd);
            return prd.Id;
        }

        public List<string?> Initialize()
        {
            if (!_repo.GetQueryable().Any())
            {
                var products = reqs.Select(reqs => reqs.ToEntity()).ToList();
                _repo.Create(products);
                return products.Select(x => x.Id).ToList();
            }
            return [];
        }

        public List<ProductResponse> ReadAll()
        {
            return _repo.GetQueryable().Select(x => x.ToResponse()).ToList();
        }

        public ProductResponse? Read(string key)
        {
            key = key.ToLower();
            var entity = _repo.GetQueryable().FirstOrDefault(x => string.Equals(
                                x.Code, key, StringComparison.OrdinalIgnoreCase));
            return entity?.ToResponse();
        }

        public bool Exist(string key)
        {
            var found = _repo.GetQueryable().FirstOrDefault(x => string.Equals(
                                x.Code, key, StringComparison.OrdinalIgnoreCase));
            return found != null;
        }

        public bool Update(ProductUpdateReq req)
        {
            var found = _repo.GetQueryable().FirstOrDefault(x => string.Equals(
                                x.Id, req.Key, StringComparison.OrdinalIgnoreCase)
                              || string.Equals(x.Code, req.Key, StringComparison.OrdinalIgnoreCase));
            if (found == null) return false;

            found.Copy(req);
            found.LastUpdated = DateTime.Now;
            return _repo.Update(found);
        }

        public bool Delete(string key)
        {
            var found = _repo.GetQueryable().FirstOrDefault(x => string.Equals(
                                x.Id, key, StringComparison.OrdinalIgnoreCase)
                              || string.Equals(x.Code, key, StringComparison.OrdinalIgnoreCase));
            if (found == null) return false;
            return _repo.Delete(found.Id!);
        }
    }
}
