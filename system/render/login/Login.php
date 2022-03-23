<?php

namespace render\login;

use framework\entities\users\UsersService;
use render\defaultBuild\BaseRenderBuilder;

class Login extends BaseRenderBuilder
{
    private UsersService $_usersService;
    public function __construct()
    {
        parent::__construct();
        $this->_usersService = new UsersService();
    }
    public function defaultParams()
    {
        return [
            
        ];
    }
    public function build()
    {
        return "<div>hi</div>";
    }
}
