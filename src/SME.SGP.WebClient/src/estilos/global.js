import { createGlobalStyle } from 'styled-components';
import { Base } from '../componentes/colors';

export default createGlobalStyle`
  @import url('https://fonts.googleapis.com/css?family=Roboto:400,700&display=swap');

  *, *:before, *:after {
    box-sizing: border-box;
    margin: 0;
    outline: 0;
    padding: 0;
  }
  *:focus {
    box-shadow: none;
  }
  html, body, #root {
    font-family: 'FontAwesome', 'Roboto', sans-serif !important;
    font-stretch: normal;
    height: 100%;
    letter-spacing: normal;
    line-height: normal;
  }
  body {
    -webkit-font-smoothing: antialiased;
    background: ${Base.CinzaFundo} !important;
    overflow-x: hidden;
  }
  button {
    cursor: pointer;
  }
  .fonte-10 {
    font-size: 10px !important;
  }
  .fonte-12 {
    font-size: 12px !important;
  }
  .fonte-13 {
    font-size: 13px !important;
  }
  .fonte-14 {
    font-size: 14px !important;
  }
  .fonte-16 {
    font-size: 16px !important;
  }

  .ant-select-dropdown-menu-item:hover {
    background-color: ${Base.Roxo}  !important;
    color: #ffffff;
  }

  .ant-select-dropdown-menu-item-selected {
    background-color:  ${Base.Roxo}  !important;
    color: #ffffff !important;
  }

  .desabilitar-elemento {
    pointer-events: none !important;
    opacity: 0.6 !important;
  }

  @media (max-width: 544px) {

    .hidden-xs-down{
      display: none !important;
    }

   }

  .p-l-5{
    padding-left: 5px !important;
  }

  .p-r-5{
    padding-right: 5px !important;
  }

  .m-r-10{
    margin-right: 10px !important;
  }

  .m-t-10{
    margin-top: 10px !important;
  }

  .m-b-10{
    margin-bottom: 10px !important;
  }

  .m-r-0{
    margin-rigth: 0px !important;
  }

  .m-l-0{
    margin-left: 0px !important;
  }

  .m-t-0{
    margin-top: 0px !important;
  }

  .m-b-0{
    margin-bottom: 0px !important;
  }

  .p-r-0{
    padding-right: 0px !important;
  }

  .p-l-0{
    padding-left: 0px !important;
  }

  .p-t-0{
    padding-top: 0px !important;
  }

  .p-b-0{
    padding-bottom: 0px !important;
  }

/*MENU*/

.menuItem{
    color: ${Base.CinzaMenuItem} !important;
  }

  .ant-menu-inline .ant-menu-item:not(:first-child), .ant-menu-vertical.ant-menu-sub > .ant-menu-item:not(:first-child){
    margin-bottom: 0px !important;
    border-top: 1px solid ${Base.RoxoClaro} !important;
  }

  .ant-menu-vertical.ant-menu-sub > .ant-menu-dark{
    background: white !important;
  }

  .ant-menu-vertical.ant-menu-sub >.ant-menu-item-selected, .ant-menu-vertical.ant-menu-sub > .ant-menu-item-selected{
    background: ${Base.CinzaMenu} !important;
    border-bottom-width: 8px;
    padding-left: 32px !important;
    border-left: solid ${Base.RoxoClaro} 8px !important;
  }

  .ant-menu-item {
    padding-left: 34px !important;
    font-size: 12px !important;
    padding-left: 40px !important;
  }

  .ant-menu-inline.ant-menu-sub{
    background: ${Base.Branco} !important;
  }

  .ant-menu-item, .ant-menu-submenu-open {
    background: ${Base.Branco} !important;
  }

  .ant-menu-sub{
    max-height: 160px;
    box-shadow: 2px 5px 6px rgba(50,50,50,0.77) !important;
    -webkit-box-shadow: 2px 5px 6px rgba(50,50,50,0.77) !important;
    -moz-box-shadow: 2px 5px 6px rgba(50,50,50,0.77) !important;
    overflow: auto;
    ::-webkit-scrollbar {
      width: 10px;
      position: absolute;
      background:${Base.Branco} !important;
      border-top-right-radius: 4px;
      border-bottom-right-radius: 4px;
    }
    ::-webkit-scrollbar-thumb {
      background: #dad7d7;
      border-radius: 4px;
    }
  }

  .ant-menu-inline, .ant-menu-submenu-title, .ant-menu-item{
    margin-bottom: 0px !important;
    margin-top: 0px !important;
    top: 0;
  }

  .ant-menu-submenu-popup{
    position: fixed !important;
  }

  .ant-modal-footer {
    border-top: 0px !important;
  }

  .ant-modal-title{
    font-size: 24px !important;
  }
`;
