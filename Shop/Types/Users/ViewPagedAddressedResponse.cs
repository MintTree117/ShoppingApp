namespace Shop.Types.Users;

public readonly record struct ViewPagedAddressesResponse(
    int TotalCount,
    List<AddressModel> Addresses );