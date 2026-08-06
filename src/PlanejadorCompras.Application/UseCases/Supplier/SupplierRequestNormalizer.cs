using PlanejadorCompras.Application.Features.Suppliers.Contracts;
using System.Net.Mail;
using PlanejadorCompras.Application.Exceptions;
using PlanejadorCompras.Domain.Entities;

namespace PlanejadorCompras.Application.UseCases.Supplier;

internal static class SupplierRequestNormalizer
{
    internal static NormalizedSupplierInput Normalize(SupplierRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = request.Name.Trim();
        var cnpj = DigitsOnly(request.Cnpj);
        var postalCode = DigitsOnly(request.Address?.PostalCode);
        var phone = NormalizePhone(request.Contact?.Phone);
        var email = NormalizeOptional(request.Contact?.Email)?.ToLowerInvariant();

        if (name.Length is < 1 or > 100)
        {
            throw new BadRequestException(
                "Informe um nome de fornecedor com ate 100 caracteres.",
                "supplier_invalid_name");
        }

        if (cnpj is not null && !IsValidCnpj(cnpj))
        {
            throw new BadRequestException(
                "Informe um CNPJ valido.",
                "supplier_invalid_cnpj");
        }

        if (postalCode is not null && postalCode.Length != 8)
        {
            throw new BadRequestException(
                "Informe um CEP com 8 digitos.",
                "supplier_invalid_postal_code");
        }

        if (email is not null
            && (!MailAddress.TryCreate(email, out var parsedEmail)
                || !string.Equals(parsedEmail.Address, email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new BadRequestException(
                "Informe um e-mail de contato valido.",
                "supplier_invalid_email");
        }

        if (phone is not null && (phone.Length is < 10 or > 13))
        {
            throw new BadRequestException(
                "Informe um telefone com DDD.",
                "supplier_invalid_phone");
        }

        var address = SupplierAddress.Create(
            request.Address?.Street,
            request.Address?.City,
            postalCode);
        var contact = SupplierContact.Create(email, phone);

        return new NormalizedSupplierInput(name, cnpj, address, contact);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? DigitsOnly(string? value)
    {
        var normalized = NormalizeOptional(value);
        return normalized is null
            ? null
            : new string(normalized.Where(char.IsDigit).ToArray());
    }

    private static string? NormalizePhone(string? value) => DigitsOnly(value);

    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
        {
            return false;
        }

        var firstWeights = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var secondWeights = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        return CalculateDigit(cnpj, firstWeights) == cnpj[12] - '0'
            && CalculateDigit(cnpj, secondWeights) == cnpj[13] - '0';
    }

    private static int CalculateDigit(string value, IReadOnlyList<int> weights)
    {
        var sum = 0;

        for (var index = 0; index < weights.Count; index++)
        {
            sum += (value[index] - '0') * weights[index];
        }

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}

internal sealed record NormalizedSupplierInput(
    string Name,
    string? Cnpj,
    SupplierAddress? Address,
    SupplierContact? Contact);
