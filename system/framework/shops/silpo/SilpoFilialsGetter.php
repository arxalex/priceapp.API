<?php

namespace framework\shops\silpo;

use framework\database\NumericHelper;
use framework\entities\filials\Filial;
use framework\entities\filials\FilialsService;

class SilpoFilialsGetter
{
    private FilialsService $_fillialsService;
    public function __construct()
    {
        $this->_fillialsService = new FilialsService();
    }

    public function getFilials(
        string $token =
        "Token eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIwNjJiM2Y1Ni0xZTJhLTQ4MzktYmVkYS04MTNmYzMwZmU1NmQiLCJ0eXAiOiJBY2Nlc3MiLCJzdWIiOiIwODQwY2NlOC03NWRjLTQyZjUtODUwNy00MjE2OGMyZjJhMjMiLCJmel9pdiI6Ilp3MmR3Y2RmVS9PbWJNeVJCblN2QlE9PSIsImZ6X3VzZXIiOiJBbFdlZkpjaWFBUkl5VStxamlZQ0lBPT0iLCJleHAiOjE2NDg0NjQyNDQsImlzcyI6ImZ6Z2xvYmFsL2FwaSJ9.O2cXrTv1xiuhwTOA3mVXJDyZ7Wzeq2plyW62OzRzarI"
    ): array {

        $curl = curl_init();

        curl_setopt_array($curl, array(
            CURLOPT_URL => 'https://api.sm.silpo.ua/api/2.0/exec/FZGlobal/',
            CURLOPT_RETURNTRANSFER => true,
            CURLOPT_ENCODING => '',
            CURLOPT_MAXREDIRS => 10,
            CURLOPT_TIMEOUT => 0,
            CURLOPT_FOLLOWLOCATION => true,
            CURLOPT_HTTP_VERSION => CURL_HTTP_VERSION_1_1,
            CURLOPT_CUSTOMREQUEST => 'POST',
            CURLOPT_POSTFIELDS => '{"method": "GetFilials"}',
            CURLOPT_HTTPHEADER => array(
                'Authorization: Token eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiIwNjJiM2Y1Ni0xZTJhLTQ4MzktYmVkYS04MTNmYzMwZmU1NmQiLCJ0eXAiOiJBY2Nlc3MiLCJzdWIiOiIwODQwY2NlOC03NWRjLTQyZjUtODUwNy00MjE2OGMyZjJhMjMiLCJmel9pdiI6Ilp3MmR3Y2RmVS9PbWJNeVJCblN2QlE9PSIsImZ6X3VzZXIiOiJBbFdlZkpjaWFBUkl5VStxamlZQ0lBPT0iLCJleHAiOjE2NDg0NjQyNDQsImlzcyI6ImZ6Z2xvYmFsL2FwaSJ9.O2cXrTv1xiuhwTOA3mVXJDyZ7Wzeq2plyW62OzRzarI',
                'Content-Type: application/json'
            ),
        ));

        $response = curl_exec($curl);

        curl_close($curl);
        echo $response;

        /*

        $url = 'https://api.sm.silpo.ua/api/2.0/exec/FZGlobal/';
        $data = json_encode([
            'method' => 'GetFilials'
        ]);

        $options = [
            'http' => [
                'header' => "authorization: $token\r\n" .
                    "Content-Type: application/json;charset=UTF-8\r\n",
                'method'  => 'POST',
                'content' => $data
            ]
        ];
        $context  = stream_context_create($options);
        $response = file_get_contents($url, false, $context)
        */
        $result = json_decode($response);

        return $result->filials;
    }
    public function convertFilial(object $filialSilpo): Filial
    {
        return new Filial(
            null,
            1,
            $filialSilpo->filialId,
            $filialSilpo->cityName,
            $filialSilpo->regionName != "НЕ ВСТАНОВЛЕНИЙ" ? $filialSilpo->regionName : "Киъвська обл.",
            $filialSilpo->streetName,
            $filialSilpo->houseNumber,
            NumericHelper::toFloat($filialSilpo->xCoord),
            NumericHelper::toFloat($filialSilpo->yCoord),
            $filialSilpo->filialName
        );
    }
}
