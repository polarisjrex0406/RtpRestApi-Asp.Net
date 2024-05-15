import { useCallback, useEffect, useContext } from 'react';

import {
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  EllipsisOutlined,
  RedoOutlined,
  ArrowRightOutlined,
  ArrowLeftOutlined,
} from '@ant-design/icons';
import { Dropdown, Table, Button, Input, Spin } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';

import { useSelector, useDispatch } from 'react-redux';
import { crud } from '@/redux/crud/actions';
import { selectListItems } from '@/redux/crud/selectors';
import { selectCreatedItem } from '@/redux/erp/selectors';
import useLanguage from '@/locale/useLanguage';
import { dataForTable } from '@/utils/dataStructure';
import { useDate } from '@/settings';

import { generate as uniqueId } from 'shortid';

import { useCrudContext } from '@/context/crud';
import { selectLangDirection } from '@/redux/translate/selectors';

import Card from './Card';
import { Tag, Row, Col } from 'antd';
import { useNavigate } from 'react-router-dom';
import { erp } from '@/redux/erp/actions';

function AddNewItem({ config }) {
  const { crudContextAction } = useCrudContext();
  const { collapsedBox, panel } = crudContextAction;
  const { ADD_NEW_ENTITY } = config;

  const navigate = useNavigate();

  const handelClick = () => {
    const pathSegments = window.location.pathname.split('/');
    const chatIdFromUrl = pathSegments[pathSegments.length - 1];
    // dispatch(erp.currentItem({ data: record }));
    navigate(`/template/create/${chatIdFromUrl}`);
    // panel.open();
    // collapsedBox.close();
  };

  return (
    <Button onClick={handelClick} type="primary">
      {ADD_NEW_ENTITY}
    </Button>
  );
}
export default function CardBox({ config, extra = [] }) {
  const filterText = useContext('FilterTextContext');

  let { entity, dataTableColumns, DATATABLE_TITLE, fields, searchConfig } = config;
  const { crudContextAction } = useCrudContext();
  const { panel, collapsedBox, modal, readBox, editBox, advancedBox } = crudContextAction;
  const translate = useLanguage();
  const { dateFormat } = useDate();

  const items = [
    {
      label: translate('Show'),
      key: 'read',
      icon: <EyeOutlined />,
    },
    {
      label: translate('Edit'),
      key: 'edit',
      icon: <EditOutlined />,
    },
    ...extra,
    {
      type: 'divider',
    },

    {
      label: translate('Delete'),
      key: 'delete',
      icon: <DeleteOutlined />,
    },
  ];

  const handleRead = (record) => {
    handleViewTemplates(record);
  };

  function handleEdit(record) {
    dispatch(crud.currentItem({ data: record }));
    dispatch(crud.currentAction({ actionType: 'update', data: record }));
    navigate(`/${entity.toLowerCase()}/update/${record._id}`);
  }

  function handleDelete(record) {
    dispatch(crud.currentAction({ actionType: 'delete', data: record }));
    modal.open();
  }

  function handleUpdatePassword(record) {
    dispatch(crud.currentItem({ data: record }));
    dispatch(crud.currentAction({ actionType: 'update', data: record }));
    advancedBox.open();
    panel.open();
    collapsedBox.open();
  }

  const navigate = useNavigate();
  function handleViewTemplates(record) {
    dispatch(erp.currentItem({ data: record }));
    navigate(`/${entity.toLowerCase()}/read/${record._id}`);
  }

  const { isSuccess, result: resultDup, isLoading: dupIsLoad } = useSelector(selectCreatedItem);
  function handleDuplicate(record) {
    dispatch(erp.currentItem({ data: record }));
    record.topic = record.topicId;
    record.name = "Copy of " + record.name;
    delete record._id;
    dispatch(erp.create({ entity, jsonData: record }));
  }

  useEffect(() => {
    if (isSuccess) {
      dispatch(erp.resetAction({ actionType: 'create' }));
      navigate(`/${entity.toLowerCase()}/update/${resultDup._id}`);
    }
    return () => { };
  }, [isSuccess]);

  let dispatchColumns = [];
  if (fields) {
    dispatchColumns = [...dataForTable({ fields, translate, dateFormat })];
  } else {
    dispatchColumns = [...dataTableColumns];
  }

  dataTableColumns = [
    ...dispatchColumns,
    {
      title: '',
      key: 'action',
      fixed: 'right',
      render: (_, record) => (
        <Dropdown
          menu={{
            items,
            onClick: ({ key }) => {
              switch (key) {
                case 'read':
                  handleRead(record);
                  break;
                case 'edit':
                  handleEdit(record);
                  break;

                case 'delete':
                  handleDelete(record);
                  break;
                case 'updatePassword':
                  handleUpdatePassword(record);
                  break;

                default:
                  break;
              }
              // else if (key === '2')handleCloseTask
            },
          }}
          trigger={['click']}
        >
          <EllipsisOutlined
            style={{ cursor: 'pointer', fontSize: '24px' }}
            onClick={(e) => e.preventDefault()}
          />
        </Dropdown>
      ),
    },
  ];

  const { result: listResult, isLoading: listIsLoading } = useSelector(selectListItems);

  const { pagination, items: dataSource } = listResult;

  const dispatch = useDispatch();

  const handelDataTableLoad = useCallback((pagination) => {
    const options = { page: pagination.current || 1, items: pagination.pageSize || 10 };
    dispatch(crud.list({ entity, options }));
  }, []);

  const filterTable = (e) => {
    const value = e.target.value;
    const options = { q: value, fields: searchConfig?.searchFields || '' };
    dispatch(crud.list({ entity, options }));
  };

  const dispatcher = () => {
    dispatch(crud.list({ entity }));
  };

  useEffect(() => {
    const controller = new AbortController();
    dispatcher();
    return () => {
      controller.abort();
    };
  }, []);

  const langDirection = useSelector(selectLangDirection);

  return (
    <>
      <PageHeader
        onBack={() => {
          navigate(`/topic`);
          // window.history.back();
        }}
        backIcon={langDirection === "rtl" ? <ArrowRightOutlined /> : <ArrowLeftOutlined />}
        title={DATATABLE_TITLE}
        ghost={false}
        extra={[
          <Input
            key={`searchFilterDataTable}`}
            onChange={filterTable}
            placeholder={translate('search')}
            allowClear
          />,
          <Button onClick={handelDataTableLoad} key={`${uniqueId()}`} icon={<RedoOutlined />}>
            {translate('Refresh')}
          </Button>,

          <AddNewItem key={`${uniqueId()}`} config={config} />,
        ]}
        style={{
          padding: '20px 0px',
          direction: langDirection,
          position: 'fixed',
          top: '0px',
        }}
      ></PageHeader>

      {(() => {
        const cards = [];
        const pathSegments = window.location.pathname.split('/');
        const chatIdFromUrl = pathSegments[pathSegments.length - 1];
        const sortedDataSource = [...dataSource];

        if (sortedDataSource.length > 0 && sortedDataSource[0].name) {
          sortedDataSource.sort((a, b) => a.name.localeCompare(b.name));
        }
        for (const i in sortedDataSource) {
          if (sortedDataSource[i].topic && sortedDataSource[i].topic._id == chatIdFromUrl) {
            cards.push(<Card
              key={`${uniqueId()}`}
              data={sortedDataSource[i]}
              isLoading={listIsLoading}
              onClickViewTemplates={handleViewTemplates}
              onClickRead={handleRead}
              onClickEdit={handleEdit}
              onClickDelete={handleDelete}
              onClickDuplicate={handleDuplicate} />);
          }
        }
        if (!listIsLoading && cards.length === 0) {
          cards.push(
            <div key={`${uniqueId()}`} style={{ textAlign: 'center', position: 'absolute', top: '50%', left: '50%' }}>
              <h1 style={{ display: 'inline' }}>No Results</h1>
            </div>
          );
        }
        return <Row gutter={[32, 32]}>
          {!listIsLoading ? cards : <div className="centerAbsolute"><Spin size="large" /></div>}
        </Row>;
      })()}
    </>
  );
}
