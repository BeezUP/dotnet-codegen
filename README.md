## dotnet-codegen

A global tool to execute handlebars templates in order to generate code (think about swagger/openapi/graphql ...)

### Installation

Installation is very easy. Just run this command and the tool will be installed. 

`dotnet tool install --global Dotnet.CodeGen`

### Usage

`dotnet-codegen <Open_Api_File_Uri> <Output_Folder> <Template_Folder1> [Template_Folder2] [Template_Folder3] [...]`

example :
`dotnet-codegen "https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v2.0/json/petstore-minimal.json" "output/folder" "my/template"`

# Custom handlebars helper

You can find additional custom helper in this folder : https://github.com/BeezUP/dotnet-codegen/tree/master/Dotnet.CodeGen/CustomHandlebars/Helpers
Use cases are documented (and tested) thought `HandlebarsHelperSpecification` attributes 

| Helper | Input document | Handlebars template | Result |
|--------|----------------|---------------------|--------|
| each_with_sort | `"[{t: 'c'}, {t: 'a'}, {t: 'b'}]"` | `"{{#each .}}{{t}}{{/each}}"` | `"cab"` |
| each_with_sort | `"[{t: 'c'}, {t: 'a'}, {t: 'b'}]"` | `"{{#each_with_sort . 't'}}{{#each .}}{{t}}{{/each}}{{/each_with_sort}}"` | `"abc"` |
| each_with_sort | `"[]"` | `"{{#each_with_sort . .}}{{/each_with_sort}}"` | `""` |
| each_with_sort | `"{ a : {}, b : {} }"` | `"{{#each_with_sort .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort}}"` | `"ab"` |
| each_with_sort | `"{ b : {}, a : {} }"` | `"{{#each_with_sort .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort}}"` | `"ab"` |
| each_with_sort | `"\r\n{\r\n  "swagger": "2.0",\r\n    "info": {\r\n        "title": "Marketplace Gateway API - Feeds",\r\n      ..."` | `"{{#each_with_sort parameters}}{{#each .}}{{@key}},{{/each}}{{/each_with_sort}}"` | `"accountIdParameter,credentialParameter,feedTypeParameter,marketplaceBusinessCodeParameter,publicationIdParameter,"` |
| each_with_sort_inv | `"[{t: 'c'}, {t: 'a'}, {t: 'b'}]"` | `"{{#each .}}{{t}}{{/each}}"` | `"cab"` |
| each_with_sort_inv | `"[{t: 'c'}, {t: 'a'}, {t: 'b'}]"` | `"{{#each_with_sort_inv . 't'}}{{#each .}}{{t}}{{/each}}{{/each_with_sort_inv}}"` | `"cba"` |
| each_with_sort_inv | `"[]"` | `"{{#each_with_sort_inv . .}}{{/each_with_sort_inv}}"` | `""` |
| each_with_sort_inv | `"{ a : {}, b : {} }"` | `"{{#each_with_sort_inv .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort_inv}}"` | `"ba"` |
| each_with_sort_inv | `"{ b : {}, a : {} }"` | `"{{#each_with_sort_inv .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort_inv}}"` | `"ba"` |
| each_with_sort_inv | `"\r\n{\r\n  "swagger": "2.0",\r\n    "info": {\r\n        "title": "Marketplace Gateway API - Feeds",\r\n      ..."` | `"{{#each_with_sort_inv parameters}}{{#each .}}{{@key}},{{/each}}{{/each_with_sort_inv}}"` | `"publicationIdParameter,marketplaceBusinessCodeParameter,feedTypeParameter,credentialParameter,accountIdParameter,"` |
| get/set | `"{}"` | `"{{set 'key', 'value'}}{{get 'key'}}"` | `"value"` |
| get/set | `"{ key: 'value' }"` | `"{{set 'k', . }}{{#with_get 'k'}}{{key}}{{/with_get}}"` | `"value"` |
| get/set | `"{ key: 'value' }"` | `"{{#with_set 'key', .key }}{{get 'key'}}{{/with_set}}{{get 'key'}}"` | `"value"` |
| get/set | `"{}"` | `"{{set 'key', '42' }}{{get 'key'}}{{clear 'key'}}{{get 'key'}}"` | `"42"` |
| if_array_contains | `"\r\n        {\r\n            'type' : 'object',\r\n            'required' : [ 'errorMeSSage', 'test' ],\r\n ..."` | `"{{#if_array_contains required 'errorMessage'}}OK{{else}}NOK{{/if_array_contains}}"` | `"OK"` |
| if_array_contains | `"\r\n        {\r\n            'type' : 'object',\r\n            'required' : [ 'errorMeSSage', 'test' ],\r\n ..."` | `"{{#if_array_contains required 'test'}}OK{{else}}NOK{{/if_array_contains}}"` | `"OK"` |
| if_array_contains | `"\r\n        {\r\n            'type' : 'object',\r\n            'required' : [ 'errorMeSSage', 'test' ],\r\n ..."` | `"{{#if_array_contains required 'notFound'}}OK{{else}}NOK{{/if_array_contains}}"` | `"NOK"` |
| if_array_contains | `"\r\n        {\r\n            'type' : 'object',\r\n            'required' : [ 'errorMeSSage', 'test' ],\r\n ..."` | `"{{#each properties}}{{#if_array_contains ../required @key}}{{type}}{{else}}{{/if_array_contains}}{{/each}}"` | `"string"` |
| if_empty | `"{}"` | `"{{#if_empty ''}}OK{{else}}{{/if_empty}}"` | `"OK"` |
| if_empty | `"{}"` | `"{{#if_empty 'test'}}OK{{else}}NOK{{/if_empty}}"` | `"NOK"` |
| if_equals | `"{}"` | `"{{#if_equals 'test' 'teSt'}}OK{{else}}{{/if_equals}}"` | `"OK"` |
| if_equals | `"{ a: '42', b: 42 }"` | `"{{#if_equals a ./b }}OK{{else}}{{/if_equals}}"` | `"OK"` |
| if_equals | `"{}"` | `"{{#if_equals 'test' 'NO'}}OK{{else}}NOK{{/if_equals}}"` | `"NOK"` |
| if_not_empty | `"{}"` | `"{{#if_not_empty ''}}{{else}}OK{{/if_not_empty}}"` | `"OK"` |
| if_not_empty | `"{}"` | `"{{#if_not_empty 'test'}}NOK{{else}}OK{{/if_not_empty}}"` | `"NOK"` |
| if_not_equals | `"{}"` | `"{{#if_not_equals 'test' 'teSt'}}{{else}}NOK{{/if_not_equals}}"` | `"NOK"` |
| if_not_equals | `"{ a: '42', b: 42 }"` | `"{{#if_not_equals a ./b }}{{else}}NOK{{/if_not_equals}}"` | `"NOK"` |
| if_not_equals | `"{}"` | `"{{#if_not_equals 'test' 'NO'}}OK{{else}}NOK{{/if_not_equals}}"` | `"OK"` |
| one_line | `"{}"` | `"{{#one_line}} {{/one_line}}"` | `""` |
| one_line | `"{}"` | `"{{#one_line}} \n {{/one_line}}"` | `""` |
| one_line | `"{}"` | `"{{#one_line}}\n {{/one_line}}"` | `""` |
| one_line | `"{}"` | `"{{#one_line}}\n{{/one_line}}"` | `""` |
| one_line | `"{}"` | `"{{#one_line}} \r\n {{/one_line}}"` | `""` |
| one_line | `"{}"` | `"{{#one_line}}\r\n{{/one_line}}"` | `""` |
| one_line | `"{}"` | `"{{#one_line}} test{{/one_line}}"` | `"test"` |
| one_line | `"{}"` | `"{{#one_line}} a \n z {{/one_line}}"` | `"a z"` |
| one_line | `"{}"` | `"{{#one_line}}a\n z{{/one_line}}"` | `"a z"` |
| one_line | `"{}"` | `"{{#one_line}}a\nz{{/one_line}}"` | `"a z"` |
| one_line | `"{}"` | `"{{#one_line}}a \r\n z{{/one_line}}"` | `"a z"` |
| one_line | `"{}"` | `"{{#one_line}}a \r\n \r\n \r\nz{{/one_line}}"` | `"a z"` |
| one_line | `"{}"` | `"{{#one_line}}test\r\n\r\n\r\ntest{{/one_line}}"` | `"test test"` |
| one_line | `"{}"` | `"{{#one_line}}{{/one_line}}"` | `""` |
| one_line | `"{}"` | `"{{#one_line}}   test {{/one_line}}"` | `"test"` |
| one_line | `"{}"` | `"{{#one_line 5}}test{{/one_line}}"` | `"     test"` |
| ref_resolve | `"\r\n{\r\n  "swagger": "2.0",\r\n    "info": {\r\n        "title": "Marketplace Gateway API - Feeds",\r\n      ..."` | `"{{#each paths}}{{#each this}}{{#each parameters}}{{#ref_resolve}}{{ name }},{{/ref_resolve}}{{/each}}{{/each}}{{/each}}"` | `"marketplaceBusinessCode,marketplaceBusinessCode,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,"` |
| split_get_first | `"{ '$ref' : '/myDataType/parameters/'}"` | `"{{split_get_first ./$ref '/' }}"` | `"myDataType"` |
| split_get_last | `"{ '$ref' : '#/parameters/myDataType'}"` | `"{{split_get_last ./$ref '/' }}"` | `"myDataType"` |
| trim | `"{ test: 42 }"` | `"{{trim test}}"` | `"42"` |
| trim | `"{ test: ' 42 ' }"` | `"{{trim test}}"` | `"42"` |
| trim | `"{ test: '- aa -' }"` | `"{{trim test '-'}}"` | `" aa "` |
| trim | `"{ test: 'AA' }"` | `"{{trim test 'A'}}"` | `""` |
| trim | `"{ test: ' test ' }"` | `"{{trim test ' t'}}"` | `"es"` |
| trim_end | `"{ test: 42 }"` | `"{{trim_end test}}"` | `"42"` |
| trim_end | `"{ test: '42 ' }"` | `"{{trim_end test}}"` | `"42"` |
| trim_end | `"{ test: 'aa -' }"` | `"{{trim_end test '-'}}"` | `"aa "` |
| trim_end | `"{ test: 'AA' }"` | `"{{trim_end test 'A'}}"` | `""` |
| trim_end | `"{ test: ' test ' }"` | `"{{trim_end test ' t'}}"` | `" tes"` |
| trim_start | `"{ test: 42 }"` | `"{{trim_start test}}"` | `"42"` |
| trim_start | `"{ test: ' 42' }"` | `"{{trim_start test}}"` | `"42"` |
| trim_start | `"{ test: '- aa' }"` | `"{{trim_start test '-'}}"` | `" aa"` |
| trim_start | `"{ test: 'AA' }"` | `"{{trim_start test 'A'}}"` | `""` |
| trim_start | `"{ test: ' test ' }"` | `"{{trim_start test ' t'}}"` | `"est "` |
| trim_block | `"{}"` | `"{{#trim_block ','}},,1,2,3,4,,{{/trim_block}}"` | `"1,2,3,4"` |
| trim_block | `"{ a: '42', b: 42, c: 42 }"` | `"{{#trim_block ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block}}"` | `"a,b,c"` |
| trim_block_start | `"{}"` | `"{{#trim_block_start ','}},,1,2,3,4,,{{/trim_block_start}}"` | `"1,2,3,4,,"` |
| trim_block_start | `"{ a: '42', b: 42, c: 42 }"` | `"{{#trim_block_start ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block_start}}"` | `"a,b,c,"` |
| trim_block_end | `"{}"` | `"{{#trim_block_end ','}},,1,2,3,4,,{{/trim_block_end}}"` | `",,1,2,3,4"` |
| trim_block_end | `"{ a: '42', b: 42, c: 42 }"` | `"{{#trim_block_end ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block_end}}"` | `"a,b,c"` |
| uppercase_first_letter | `"{}"` | `"{{uppercase_first_letter .}}"` | `"{}"` |
| uppercase_first_letter | `"{ test: 42 }"` | `"{{uppercase_first_letter test}}"` | `"42"` |
| uppercase_first_letter | `"{ test: '42' }"` | `"{{uppercase_first_letter test}}"` | `"42"` |
| uppercase_first_letter | `"{ test: 'aa' }"` | `"{{uppercase_first_letter test}}"` | `"Aa"` |
| uppercase_first_letter | `"{ test: 'AA' }"` | `"{{uppercase_first_letter test}}"` | `"AA"` |
| uppercase_first_letter | `"{ test: 'AA' }"` | `"test{{uppercase_first_letter test}}"` | `"testAA"` |
| with_matching | `"{}"` | `"{{#with_matching 'test' '1' '1', '2', '2'}}{{else}}NOT FOUND{{/with_matching}}"` | `"NOT FOUND"` |
| with_matching | `"{}"` | `"{{#with_matching 'value1' 'value1' 'context1', '2', '2'}}{{.}}{{else}}NOT FOUND{{/with_matching}}"` | `"context1"` |
| with_matching | `"{ value: '42' }"` | `"{{#with_matching value '42' . }}{{value}}{{else}}NOT FOUND{{/with_matching}}"` | `"42"` |
| start_with | `"{}"` | `"{{#start_with 'test' 'test-one'}}OK{{else}}{{/start_with}}"` | `"OK"` |
| start_with | `"{one: 'test-one', two: 'one-test'}"` | `"{{#start_with 'test' one}}OK{{else}}{{/start_with}}"` | `"OK"` |

### Update

`dotnet tool update -g Dotnet.CodeGen`

### Uninstall

`dotnet tool uninstall -g Dotnet.CodeGen`
