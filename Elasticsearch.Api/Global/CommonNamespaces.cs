/****************************************************************************************************************************/
// GlobalNamespaces sınıfı dotnet'in sunduğu namespace'lerin artık sınıfların üzerinde yazılma zorunluluğunun ortadan       //
// kalkması ile kullandığımız bir yapıdır. Bu sınıf içerisine başında `global` etiketi ile eklediğimiz tüm namespace'ler    //
// artık aynı assembly içerisindeki diğer sınıflar içinde eklenmiş olacaktır. Dolayısıyla aynı namespace'i tekrar tekrar    //
// eklememize gerek yoktur. Burada eklenen her namespace için bir alan yaratıp onun altına ilgili namespace'leri eklemek    //
// bize aradığımızda daha kolay bir ulaşım sağlayacaktır.                                                                   //
//                                                                                                                          //
// ** NOT ** : EKLENEN TÜM LIBRARY LER AİT OLDUĞU ALANIN ALTINA VE ALFABETİK SIRALAMA BAZ ALINARAK EKLENMELİDİR.            //
/****************************************************************************************************************************/

/* (ELASTICSEARH) */

global using Elasticsearch.Api.Extensions;
global using Elasticsearch.Application.ServiceInterfaces;
global using Elasticsearch.Application.Services;
global using Elasticsearch.Core.Configs;
global using Elasticsearch.Core.Models;
global using Elasticsearch.Core.ResultModels.BaseResultModel;
global using Elasticsearch.Infrastructure.Repositories;
global using Elasticsearch.Infrastructure.RepositoryInterfaces;

/* (NUGET) */

global using Elastic.Clients.Elasticsearch;
global using Elastic.Transport;
global using Microsoft.AspNetCore.Mvc;
global using System.Net;
global using System.Reflection;