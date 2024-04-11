/****************************************************************************************************************************/
// GlobalNamespaces sınıfı dotnet'in sunduğu namespace'lerin artık sınıfların üzerinde yazılma zorunluluğunun ortadan       //
// kalkması ile kullandığımız bir yapıdır. Bu sınıf içerisine başında `global` etiketi ile eklediğimiz tüm namespace'ler    //
// artık aynı assembly içerisindeki diğer sınıflar içinde eklenmiş olacaktır. Dolayısıyla aynı namespace'i tekrar tekrar    //
// eklememize gerek yoktur. Burada eklenen her namespace için bir alan yaratıp onun altına ilgili namespace'leri eklemek    //
// bize aradığımızda daha kolay bir ulaşım sağlayacaktır.                                                                   //
//                                                                                                                          //
// ** NOT ** : EKLENEN TÜM LIBRARY LER AİT OLDUĞU ALANIN ALTINA VE ALFABETİK SIRALAMA BAZ ALINARAK EKLENMELİDİR.            //
/****************************************************************************************************************************/

/* (FRAMEWORK) */

/* (ELASTICSEARH) */

global using Elasticsearch.Core.Entities;
global using Elasticsearch.Core.Entities.BaseEntites;
global using Elasticsearch.Core.Models;
global using Elasticsearch.Infrastructure.Repositories.BaseRepositories;
global using Elasticsearch.Infrastructure.RepositoryInterfaces;
global using Elasticsearch.Infrastructure.RepositoryInterfaces.BaseRepositoryInterfaces;

/* (NUGET) */

global using Elastic.Clients.Elasticsearch;
global using Elastic.Clients.Elasticsearch.QueryDsl;