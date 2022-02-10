<?php

namespace framework\core\constants;

class ConstantsService{
    private array $constants = [
        'DB_host' => 'localhost',
        'DB_dbname' => 'arxalexc_priceapp',
        'DB_username' => 'arxalexc_priceapp',
        'DB_password' => 'WGnxAW3SWKjkhMN'
    ];
    public function __construct()
    {
        
    }
    public function getConstant(string $key){
        return $this->constants[$key];
    }
}