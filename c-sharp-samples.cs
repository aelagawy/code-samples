// utils functions

public static IEnumerable<IEnumerable<T>> GroupAdjacentBy<T>(
    this IEnumerable<T> source, Func<T, T, bool> predicate)
{
    using (var e = source.GetEnumerator())
    {
        if (e.MoveNext())
        {
            var list = new List<T> { e.Current };
            var pred = e.Current;
            while (e.MoveNext())
            {
                if (predicate(pred, e.Current))
                {
                    list.Add(e.Current);
                }
                else
                {
                    yield return list;
                    list = new List<T> { e.Current };
                }
                pred = e.Current;
            }
            yield return list;
        }
    }
}

public static List<IEnumerable<long>> SplitRanges(
    this List<IEnumerable<long>> source, long threshold)
{
    var _list = new List<IEnumerable<long>>();

    foreach (var range in source)
    {
        //skip if single
        if (range.Count() == 0)
        {
            _list.Add(range);
            continue;
        }

        //skip if under threshold
        if (range.Max() - range.Min() + 1 <= threshold)
        {
            _list.Add(range);
            continue;
        }

        //execute
        var _count = Math.Ceiling((double)(range.Max() - range.Min() + 1) / threshold);

        for (int i = 0; i < _count; i++)
        {

            var _start = i * threshold + range.Min();

            //last iteration
            if (i + 1 == _count)
            {
                if (_start == range.Max())
                {
                    _list.Add(new List<long>() { range.Max() });
                    continue;

                }
                _list.Add(new List<long>() { _start, range.Max() });
                continue;
            }

            _list.Add(new List<long>() { _start, _start + threshold - 1 });
        }
    }
    return _list;
}


//Controller

namespace x.WebAPI.Controllers
{
    [Route("lookups/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EarlyRetirementController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        //[AuthorizeEnum(SecurityRole.LOOKUPS_READ)]
        public async Task<ActionResult<CountriesVm>> Get(Language? lang = null)
        {
            return await this._mediator.Send(new CountriesQuery(lang));
        }

        [HttpGet("{id}")]
        //[AuthorizeEnum(SecurityRole.LOOKUPS_READ)]
        public async Task<IActionResult> GetById(int id, Language? lang = null)
        {
            var result = await this._mediator.Send(new CountryQuery(lang) { Id = id });
            if (result == default)
            {
            
            }
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        [AuthorizeEnum(SecurityRole.LOOKUPS_CREATE)]
        public async Task<IActionResult<int>> Create(CreateCountryCommand cmd)
        {
            return await this._mediator.Send(cmd);
        }

        [HttpPut]
        [AuthorizeEnum(SecurityRole.LOOKUPS_UPDATE)]
        public async Task<ActionResult<bool>> Update(UpdateCountryCommand cmd)
        {
            return await this._mediator.Send(cmd);
        }

        [HttpDelete("{id}")]
        [AuthorizeEnum(SecurityRole.LOOKUPS_DELETE)]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await this._mediator.Send(new DeleteCountryCommand { Id = id });
        }
    }
}
