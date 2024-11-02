// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonServiceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;
using Kephas.Serialization.Json;
using Kephas.Testing;
using Kephas.Tests.Serialization.Json.Model;
using Xunit;

namespace Kephas.Tests.Serialization.Json;

public class JsonServiceTest : IntegrationTestBase<JsonServiceTest>
{
    private readonly IJsonService jsonService = new JsonService(new JsonSerializerOptionsProvider());
    
    [Fact]
    public void Serialize_ErrorModel_ArgumentException()
    {
        var responseModel = new ErrorModel<ArgumentException>
        {
            CopyrightNotice = "mine",
            Exception = new ArgumentException("bad argument", "arg"),
        };

        var json = jsonService.Serialize(responseModel);
        Assert.Equal("{\"copyrightNotice\":\"mine\",\"durationInMilliseconds\":0,\"exception\":{\"message\":\"bad argument (Parameter \\u0027arg\\u0027)\",\"paramName\":\"arg\",\"data\":{},\"hResult\":-2147024809},\"statusCode\":0,\"errorId\":\"00000000-0000-0000-0000-000000000000\"}", json);
    }
    
    [Fact]
    public void Deserialize_ErrorModel_non_generic_ArgumentException_as_ExceptionData()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"exception\":{\"message\":\"bad argument (Parameter \\u0027arg\\u0027)\",\"paramName\":\"arg\",\"targetSite\":null,\"data\":{},\"innerException\":null,\"helpLink\":null,\"source\":null,\"hResult\":-2147024809,\"stackTrace\":null},\"message\":null,\"statusCode\":0,\"errorId\":\"00000000-0000-0000-0000-000000000000\"}";
        var errorModel = (ErrorModel<ExceptionData>)jsonService.Deserialize(json, typeof(ErrorModel<ExceptionData>))!;
        
        Assert.Equal("bad argument (Parameter 'arg')", errorModel.Exception!.Message);
    }
    
    [Fact]
    public void Deserialize_ErrorModel_ArgumentException_as_ExceptionData()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"exception\":{\"message\":\"bad argument (Parameter \\u0027arg\\u0027)\",\"paramName\":\"arg\",\"targetSite\":null,\"data\":{},\"innerException\":null,\"helpLink\":null,\"source\":null,\"hResult\":-2147024809,\"stackTrace\":null},\"message\":null,\"statusCode\":0,\"errorId\":\"00000000-0000-0000-0000-000000000000\"}";
        var errorModel = jsonService.Deserialize<ErrorModel<ExceptionData>>(json);
        
        Assert.Equal("bad argument (Parameter 'arg')", errorModel.Exception!.Message);
    }
    
    [Fact]
    public async Task DeserializeAsync_ErrorModel_nono_generic_ArgumentException_as_ExceptionData()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"exception\":{\"message\":\"bad argument (Parameter \\u0027arg\\u0027)\",\"paramName\":\"arg\",\"targetSite\":null,\"data\":{},\"innerException\":null,\"helpLink\":null,\"source\":null,\"hResult\":-2147024809,\"stackTrace\":null},\"message\":null,\"statusCode\":0,\"errorId\":\"00000000-0000-0000-0000-000000000000\"}";
        var errorModel = (ErrorModel<ExceptionData>)(await jsonService.DeserializeAsync(json, typeof(ErrorModel<ExceptionData>)))!;
        
        Assert.Equal("bad argument (Parameter 'arg')", errorModel.Exception!.Message);
    }
    
    [Fact]
    public async Task DeserializeAsync_ErrorModel_ArgumentException_as_ExceptionData()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"exception\":{\"message\":\"bad argument (Parameter \\u0027arg\\u0027)\",\"paramName\":\"arg\",\"targetSite\":null,\"data\":{},\"innerException\":null,\"helpLink\":null,\"source\":null,\"hResult\":-2147024809,\"stackTrace\":null},\"message\":null,\"statusCode\":0,\"errorId\":\"00000000-0000-0000-0000-000000000000\"}";
        var errorModel = await jsonService.DeserializeAsync<ErrorModel<ExceptionData>>(json);
        
        Assert.Equal("bad argument (Parameter 'arg')", errorModel.Exception!.Message);
    }
    
    [Fact]
    public void Serialize_ResponseModel_string()
    {
        var responseModel = new ResponseModel<string>
        {
            Message = "OK",
            NumberOfElements = 1,
            Elements = "hi there",
        };
        var zero = "0";

        var json = jsonService.Serialize(responseModel);
        Assert.Equal($"{{\"durationInMilliseconds\":{zero},\"message\":\"OK\",\"numberOfElements\":1,\"elements\":\"hi there\"}}", json);
    }
    
    [Fact]
    public void Serialize_ResponseModel_as_interface()
    {
        IResponseModel responseModel = new ResponseModel<List<string>>
        {
            NumberOfElements = 2,
            Elements = new List<string> { "hi", "there" },
        };
        var zero = "0";

        var json = jsonService.Serialize(responseModel);
        Assert.Equal($"{{\"durationInMilliseconds\":{zero},\"numberOfElements\":2,\"elements\":[\"hi\",\"there\"]}}", json);
    }
    
    [Fact]
    public async Task SerializeAsync_ResponseModel_as_interface()
    {
        IResponseModel responseModel = new ResponseModel<List<string>>
        {
            NumberOfElements = 2,
            Elements = new List<string> { "hi", "there" },
        };
        var zero = "0";
        var json = await jsonService.SerializeAsync(responseModel);
        Assert.Equal($"{{\"durationInMilliseconds\":{zero},\"numberOfElements\":2,\"elements\":[\"hi\",\"there\"]}}", json);
    }
    
    [Fact]
    public void Serialize_ResponseModel_non_generic_as_interface()
    {
        var responseModel = new ResponseModel<List<string>>
        {
            NumberOfElements = 2,
            Elements = new List<string> { "hi", "there" },
        };
        var json = jsonService.Serialize(responseModel, typeof(IResponseModel));
        Assert.Equal($"{{\"numberOfElements\":2}}", json);
    }
    
    [Fact]
    public async Task SerializeAsync_ResponseModel_non_generic_as_interface()
    {
        var responseModel = new ResponseModel<List<string>>
        {
            NumberOfElements = 2,
            Elements = new List<string> { "hi", "there" },
        };
        var json = await jsonService.SerializeAsync(responseModel, typeof(IResponseModel));
        Assert.Equal($"{{\"numberOfElements\":2}}", json);
    }
    
    [Fact]
    public void Serialize_ResponseModel_List_of_string()
    {
        var responseModel = new ResponseModel<List<string>>
        {
            NumberOfElements = 2,
            Elements = new List<string> { "hi", "there" },
        };
        var zero = "0";
        var json = jsonService.Serialize(responseModel);
        Assert.Equal($"{{\"durationInMilliseconds\":{zero},\"numberOfElements\":2,\"elements\":[\"hi\",\"there\"]}}", json);
    }
    
    [Fact]
    public void Deserialize_ResponseModel_string()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"message\":null,\"numberOfElements\":1,\"elements\":\"hi there\"}";
        var responseModel = jsonService.Deserialize<ResponseModel<string>>(json);
        
        Assert.Equal(1, responseModel.NumberOfElements);
        Assert.Equal("hi there", responseModel.Elements);
    }
    
    [Fact]
    public void Deserialize_ResponseModel_List_of_string()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"message\":null,\"numberOfElements\":2,\"elements\":[\"hi\",\"there\"]}";
        var responseModel = jsonService.Deserialize<ResponseModel<List<string>>>(json);

        Assert.Equal(2, responseModel.NumberOfElements);
        Assert.Collection(
            responseModel.Elements!,
            s => Assert.Equal("hi", s),
            s => Assert.Equal("there", s));
    }
    
    [Fact]
    public void Deserialize_Serialize_ResponseModel_as_dynamic()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"message\":null,\"numberOfElements\":2,\"elements\":[{\"p1\":\"hi\"},{\"p1\":\"there\"}]}";
        var responseModel = jsonService.Deserialize<ResponseModel<dynamic>>(json);

        responseModel.CopyrightNotice = "Copyright Test";
        responseModel.DurationInMilliseconds = 12;
        var duration = "12";
        var newJson = jsonService.Serialize(responseModel);
        Assert.Equal($"{{\"copyrightNotice\":\"Copyright Test\",\"durationInMilliseconds\":{duration},\"numberOfElements\":2,\"elements\":[{{\"p1\":\"hi\"}},{{\"p1\":\"there\"}}]}}", newJson);
    }
    
    [Fact]
    public void Deserialize_Serialize_ResponseModel_as_dynamic_include_nulls()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"message\":null,\"numberOfElements\":2,\"elements\":[{\"p1\":\"hi\"},{\"p1\":\"there\"}]}";
        var responseModel = jsonService.Deserialize<ResponseModel<dynamic>>(json);

        responseModel.CopyrightNotice = "Copyright Test";
        responseModel.DurationInMilliseconds = 12;
        var duration = "12";
        var newJson = jsonService.Serialize(
            responseModel,
            options => options.DefaultIgnoreCondition = JsonIgnoreCondition.Never);

        Assert.Equal($"{{\"copyrightNotice\":\"Copyright Test\",\"durationInMilliseconds\":{duration},\"message\":null,\"numberOfElements\":2,\"elements\":[{{\"p1\":\"hi\"}},{{\"p1\":\"there\"}}]}}", newJson);
    }

    [Fact]
    public void Deserialize_ResponseModel_string_excess_data()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"message\":null,\"numberOfElements\":1,\"elements\":\"hi there\",\"name\":\"john doe\"}";
        var responseModel = jsonService.Deserialize<ResponseModel<string>>(json);

        Assert.NotNull(responseModel.Data);
        Assert.Equal("john doe", responseModel.Data["name"].ToString());
    }

    [Fact]
    public void Deserialize_dynamic_ToString()
    {
        var json = "{\"copyrightNotice\":null,\"durationInMilliseconds\":0,\"message\":null,\"numberOfElements\":1,\"elements\":\"hi there\",\"name\":\"john doe\"}";
        var responseModel = jsonService.Deserialize<dynamic>(json);
        var newJson = responseModel.ToString();

        Assert.Equal(json, newJson);
    }
    
    [Fact(Skip = "Named tuples do not work with System.Text.Json")]
    public void Deserialize_Serialize_named_tuples()
    {
        var elements = new List<(Guid id, int status, string? message)>
        {
            (Guid.Parse("533213ce-7a7b-445d-a466-572ceea2490a"), 200, "OK"),
            (Guid.Parse("27091f5f-e3d0-473c-b17f-f22fee68013a"), 403, "Not Authorized"),
        };

        var json = jsonService.Serialize(elements);
        Assert.Equal($"[{{\"item1\":\"533213ce-7a7b-445d-a466-572ceea2490a\",\"item2\":200,\"item3\":\"OK\"}},{{\"item1\":\"27091f5f-e3d0-473c-b17f-f22fee68013a\",\"item2\":403,\"item3\":\"Not Authorized\"}}]", json);

        var newElements = jsonService.Deserialize<IEnumerable<(Guid id, int status, string? message)>>(json);
        Assert.Collection(
            newElements,
            e => Assert.Equal((Guid.Parse("533213ce-7a7b-445d-a466-572ceea2490a"), 200, "OK"), e),
            e => Assert.Equal((Guid.Parse("27091f5f-e3d0-473c-b17f-f22fee68013a"), 403, "Not Authorized"), e));
    }
}