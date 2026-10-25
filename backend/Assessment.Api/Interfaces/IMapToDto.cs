public interface IMapToDto<out TDto> where TDto : class
{
    public TDto ToDto();
}