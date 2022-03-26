<?php

namespace endpoint\login;

use endpoint\defaultBuild\BaseEndpointBuilder;
use stdClass;

class ChangeAccount extends BaseEndpointBuilder
{
    public function __construct()
    {
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'cookie' => [],
            'old_password' => '',
            'new_password' => ''
        ];
    }
    public function build()
    {
        $result = new stdClass();

        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);

        if ($this->getParam('old_password') == "" || $this->getParam('new_password') == "") {
            http_response_code(403);
            die();
        }

        $result->statusChange = $this->_usersService->changePassword($this->getParam('cookie'), $this->getParam('old_password'), $this->getParam('new_password'));

        return $result;
    }
}
