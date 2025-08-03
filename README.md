# csharp_sample_solution


## create Empty Solution

## create WebAPI controller project

mkdir WebAPI

cd WebAPI

dotnet new webapi --use-controllers



## docker

docker build -t webapi -f WebAPI/Dockerfile .

docker build -t schedulejob -f ScheduleJob/Dockerfile .

docker run -p 8080:8080 webapi







