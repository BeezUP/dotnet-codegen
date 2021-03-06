# CodegenUP

[![Nuget CodegenUP](https://img.shields.io/nuget/v/CodegenUP?label=Nuget%20CodegenUP)](https://www.nuget.org/packages/CodegenUP)
[![Nuget CodegenUP.Engine](https://img.shields.io/nuget/v/CodegenUP.Engine?label=Nuget%20CodegenUP.Engine)](https://www.nuget.org/packages/CodegenUP.Engine)
[![Nuget CodegenUP.DocumentRefLoader](https://img.shields.io/nuget/v/CodegenUP.DocumentRefLoader?label=Nuget%20CodegenUP.DocumentRefLoader)](https://www.nuget.org/packages/CodegenUP.DocumentRefLoader)

A global tool to execute handlebars templates in order to generate code (think about swagger/openapi/graphql ...)

## Installation

Installation is very easy. Just run this command and the tool will be installed. 

`dotnet tool install --global CodegenUP`

## Usage

```
codegenup -h

Usage:  [options]

Options:
  -?|-h|--help          Show help information
  -l|--loader           Enter a schema loader type between those values [RawJson | Swagger | OpenApi | GraphQl | RawXml]
  -d|--duplicates       Enter a template duplication handling strategy between those values [Throw | KeepLast | KeepFirst]
  -s|--source           Enter a path (relative or absolute) to an source document.
  -a|--auth             Enter an authorization token to access source documents
  -o|--output           Enter the path (relative or absolute) to the output path (content will be overritten)
  -t|--template         Enter a path (relative or absolute / file or folder) to a template.
  -i|--intermediate     Enter a path (relative or absolute) to a file for intermediate 'all refs merged' output of the json document
  -c|--customhelpers    Enter a path (relative or absolute) to a folder with a custom helpers project (.csproj)
  --artifacts           Enter a path (relative or absolute) where the custom helpers builds process can output artifacts (default ./temp)
  -g|--globalparameter  Enter a global parameter value on the form key=value, it'll be available throught the global_parameterhelper

```
```bash
codegenup -s <Open_Api_File_Uri1> -o <Output_Folder> -t <Template_Folder1> [-s <Open_Api_File_Uri2>] [-t Template_Folder2] [-t Template_Folder3] [...]
````

example :

```bash
codegenup -s "https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v2.0/json/petstore-minimal.json" -o "output/folder" -t "my_template_folder"
```
## Handlebars helper

You can find additional custom helper in this folder : https://github.com/BeezUP/dotnet-codegen/tree/master/src/CodegenUP.Engine/CustomHandlebars/Helpers
Use cases are documented (and tested) thought `HandlebarsHelperSpecification` attributes 

### each_with_sort
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `[{t: 'c'}, {t: 'a'}, {t: 'b'}]` | `{{#each .}}{{t}}{{/each}}` | `cab` |
| `[{t: 'c'}, {t: 'a'}, {t: 'b'}]` | `{{#each_with_sort . 't'}}{{#each .}}{{t}}{{/each}}{{/each_with_sort}}` | `abc` |
| `[]` | `{{#each_with_sort . .}}{{/each_with_sort}}` | `` |
| `{ a : {}, b : {} }` | `{{#each_with_sort .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort}}` | `ab` |
| `{ b : {}, a : {} }` | `{{#each_with_sort .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort}}` | `ab` |
| `\r\n{\r\n  "swagger": "2.0",\r\n    "info": {\r\n        "title": "Marketplace Gateway API - Feeds",\r\n      ...` | `{{#each_with_sort parameters}}{{#each .}}{{@key}},{{/each}}{{/each_with_sort}}` | `accountIdParameter,credentialParameter,feedTypeParameter,marketplaceBusinessCodeParameter,publicationIdParameter,` |
### each_with_sort_inv
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `[{t: 'c'}, {t: 'a'}, {t: 'b'}]` | `{{#each .}}{{t}}{{/each}}` | `cab` |
| `[{t: 'c'}, {t: 'a'}, {t: 'b'}]` | `{{#each_with_sort_inv . 't'}}{{#each .}}{{t}}{{/each}}{{/each_with_sort_inv}}` | `cba` |
| `[]` | `{{#each_with_sort_inv . .}}{{/each_with_sort_inv}}` | `` |
| `{ a : {}, b : {} }` | `{{#each_with_sort_inv .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort_inv}}` | `ba` |
| `{ b : {}, a : {} }` | `{{#each_with_sort_inv .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort_inv}}` | `ba` |
| `\r\n{\r\n  "swagger": "2.0",\r\n    "info": {\r\n        "title": "Marketplace Gateway API - Feeds",\r\n      ...` | `{{#each_with_sort_inv parameters}}{{#each .}}{{@key}},{{/each}}{{/each_with_sort_inv}}` | `publicationIdParameter,marketplaceBusinessCodeParameter,feedTypeParameter,credentialParameter,accountIdParameter,` |
### get/set
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{set 'key', 'value'}}{{get 'key'}}` | `value` |
| `{ key: 'value' }` | `{{set 'k', . }}{{#with_get 'k'}}{{key}}{{/with_get}}` | `value` |
| `{ key: 'value' }` | `{{#with_set 'key', .key }}{{get 'key'}}{{/with_set}}{{get 'key'}}` | `value` |
| `{}` | `{{set 'key', '42' }}{{get 'key'}}{{clear 'key'}}{{get 'key'}}` | `42` |
### if_array_contains
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `\r\n        {\r\n            'type' : 'object',\r\n            'required' : [ 'errorMeSSage', 'test' ],\r\n ...` | `{{#if_array_contains required 'errorMessage'}}OK{{else}}NOK{{/if_array_contains}}` | `OK` |
| `\r\n        {\r\n            'type' : 'object',\r\n            'required' : [ 'errorMeSSage', 'test' ],\r\n ...` | `{{#if_array_contains required 'test'}}OK{{else}}NOK{{/if_array_contains}}` | `OK` |
| `\r\n        {\r\n            'type' : 'object',\r\n            'required' : [ 'errorMeSSage', 'test' ],\r\n ...` | `{{#if_array_contains required 'notFound'}}OK{{else}}NOK{{/if_array_contains}}` | `NOK` |
| `\r\n        {\r\n            'type' : 'object',\r\n            'required' : [ 'errorMeSSage', 'test' ],\r\n ...` | `{{#each properties}}{{#if_array_contains ../required @key}}{{type}}{{else}}{{/if_array_contains}}{{/each}}` | `string` |
### if_empty
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#if_empty ''}}OK{{else}}{{/if_empty}}` | `OK` |
| `{}` | `{{#if_empty 'test'}}OK{{else}}NOK{{/if_empty}}` | `NOK` |
### if_equals
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#if_equals 'test' 'teSt'}}OK{{else}}{{/if_equals}}` | `OK` |
| `{ a: '42', b: 42 }` | `{{#if_equals a ./b }}OK{{else}}{{/if_equals}}` | `OK` |
| `{}` | `{{#if_equals 'test' 'NO'}}OK{{else}}NOK{{/if_equals}}` | `NOK` |
### if_not_empty
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#if_not_empty ''}}{{else}}OK{{/if_not_empty}}` | `OK` |
| `{}` | `{{#if_not_empty 'test'}}NOK{{else}}OK{{/if_not_empty}}` | `NOK` |
### if_not_equals
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#if_not_equals 'test' 'teSt'}}{{else}}NOK{{/if_not_equals}}` | `NOK` |
| `{ a: '42', b: 42 }` | `{{#if_not_equals a ./b }}{{else}}NOK{{/if_not_equals}}` | `NOK` |
| `{}` | `{{#if_not_equals 'test' 'NO'}}OK{{else}}NOK{{/if_not_equals}}` | `OK` |
### one_line
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#one_line 5}}test{{/one_line}}` | `     test` |
### ref_resolve
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `\r\n{\r\n  "swagger": "2.0",\r\n    "info": {\r\n        "title": "Marketplace Gateway API - Feeds",\r\n      ...` | `{{#each paths}}{{#each this}}{{#each parameters}}{{#ref_resolve}}{{ name }},{{/ref_resolve}}{{/each}}{{/each}}{{/each}}` | `marketplaceBusinessCode,marketplaceBusinessCode,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,` |
### split_get_first
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{ '$ref' : '/myDataType/parameters/'}` | `{{split_get_first ./$ref '/' }}` | `myDataType` |
### split_get_last
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{ '$ref' : '#/parameters/myDataType'}` | `{{split_get_last ./$ref '/' }}` | `myDataType` |
### start_with
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#start_with 'test' 'test-one'}}OK{{else}}{{/start_with}}` | `OK` |
| `{}` | `{{#start_with 'test' 'one-test'}}OK{{else}}NOK{{/start_with}}` | `NOK` |
| `{one: 'test-one', two: 'one-test'}` | `{{#start_with 'test' one}}OK{{else}}{{/start_with}}` | `OK` |
| `{one: 'test-one', two: 'one-test'}` | `{{#start_with 'test' two}}OK{{else}}NOK{{/start_with}}` | `NOK` |
### trim
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{ test: 42 }` | `{{trim test}}` | `42` |
| `{ test: ' 42 ' }` | `{{trim test}}` | `42` |
| `{ test: '- aa -' }` | `{{trim test '-'}}` | ` aa ` |
| `{ test: 'AA' }` | `{{trim test 'A'}}` | `` |
| `{ test: ' test ' }` | `{{trim test ' t'}}` | `es` |
### trim_block
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#trim_block ' '}} 1,2,3,4 {{/trim_block}}` | `1,2,3,4` |
| `{}` | `{{#trim_block ','}}1,2,3,4{{/trim_block}}` | `1,2,3,4` |
| `{}` | `{{#trim_block ','}}1,2,3,4,{{/trim_block}}` | `1,2,3,4` |
| `{}` | `{{#trim_block ','}},1,2,3,4,{{/trim_block}}` | `1,2,3,4` |
| `{}` | `{{#trim_block ','}},,1,2,3,4,,{{/trim_block}}` | `1,2,3,4` |
| `{ a: '42', b: 42, c: 42 }` | `{{#trim_block ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block}}` | `a,b,c` |
### trim_block_end
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#trim_block_end}} 1,2,3,4 {{/trim_block_end}}` | ` 1,2,3,4` |
| `{}` | `{{#trim_block_end ','}}1,2,3,4{{/trim_block_end}}` | `1,2,3,4` |
| `{}` | `{{#trim_block_end ','}}1,2,3,4,{{/trim_block_end}}` | `1,2,3,4` |
| `{}` | `{{#trim_block_end ','}},1,2,3,4,{{/trim_block_end}}` | `,1,2,3,4` |
| `{}` | `{{#trim_block_end ','}},,1,2,3,4,,{{/trim_block_end}}` | `,,1,2,3,4` |
| `{ a: '42', b: 42, c: 42 }` | `{{#trim_block_end ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block_end}}` | `a,b,c` |
### trim_block_start
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#trim_block_start}} 1,2,3,4 {{/trim_block_start}}` | `1,2,3,4 ` |
| `{}` | `{{#trim_block_start ','}}1,2,3,4{{/trim_block_start}}` | `1,2,3,4` |
| `{}` | `{{#trim_block_start ','}}1,2,3,4,{{/trim_block_start}}` | `1,2,3,4,` |
| `{}` | `{{#trim_block_start ','}},1,2,3,4,{{/trim_block_start}}` | `1,2,3,4,` |
| `{}` | `{{#trim_block_start ','}},,1,2,3,4,,{{/trim_block_start}}` | `1,2,3,4,,` |
| `{ a: '42', b: 42, c: 42 }` | `{{#trim_block_start ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block_start}}` | `a,b,c,` |
### trim_end
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{ test: 42 }` | `{{trim_end test}}` | `42` |
| `{ test: '42 ' }` | `{{trim_end test}}` | `42` |
| `{ test: 'aa -' }` | `{{trim_end test '-'}}` | `aa ` |
| `{ test: 'AA' }` | `{{trim_end test 'A'}}` | `` |
| `{ test: ' test ' }` | `{{trim_end test ' t'}}` | ` tes` |
### trim_start
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{ test: 42 }` | `{{trim_start test}}` | `42` |
| `{ test: ' 42' }` | `{{trim_start test}}` | `42` |
| `{ test: '- aa' }` | `{{trim_start test '-'}}` | ` aa` |
| `{ test: 'AA' }` | `{{trim_start test 'A'}}` | `` |
| `{ test: ' test ' }` | `{{trim_start test ' t'}}` | `est ` |
### uppercase_first_letter
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{uppercase_first_letter .}}` | `{}` |
| `{ test: 42 }` | `{{uppercase_first_letter test}}` | `42` |
| `{ test: '42' }` | `{{uppercase_first_letter test}}` | `42` |
| `{ test: 'aa' }` | `{{uppercase_first_letter test}}` | `Aa` |
| `{ test: 'AA' }` | `{{uppercase_first_letter test}}` | `AA` |
| `{ test: 'AA' }` | `test{{uppercase_first_letter test}}` | `testAA` |
### with_matching
| Input document | Handlebars template | Result |
|----------------|---------------------|--------|
| `{}` | `{{#with_matching 'test' '1' '1', '2', '2'}}{{else}}NOT FOUND{{/with_matching}}` | `NOT FOUND` |
| `{}` | `{{#with_matching 'value1' 'value1' 'context1', '2', '2'}}{{.}}{{else}}NOT FOUND{{/with_matching}}` | `context1` |
| `{ value: '42' }` | `{{#with_matching value '42' . }}{{value}}{{else}}NOT FOUND{{/with_matching}}` | `42` |


## Custom Handlebars Helpers

To create custom local dotnet handlebars helpers, copy the [CustomHelpers](https://github.com/BeezUP/dotnet-codegen/tree/master/src/CodegenUP.CustomHelpers) project in any folder you'd like.

Then use the `-c` command line option to enable the dynamic compiling & loading of the helpers you created.
The project is, in itself, a unit test project with what you need to simply create unit test case by attributes placed on your custom helpers.

## Add global parameters from the command line 

Through a special helper named `global_parameter`, it's possible to pass global available parameters to your template. From the command line add a as many `-g key=value` global values and use them in your templates. 
```
{{global_parameter key}}
```
will output
```
value
```

## Update

`dotnet tool update -g CodegenUP`

### Uninstall

`dotnet tool uninstall -g CodegenUP`
