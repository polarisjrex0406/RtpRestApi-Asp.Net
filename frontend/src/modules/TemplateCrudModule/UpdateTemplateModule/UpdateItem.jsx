import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { erp } from '@/redux/erp/actions';
import { generate as uniqueId } from 'shortid';
import { selectUpdatedItem } from '@/redux/erp/selectors';

import { Form, Divider, Button } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import { CloseCircleOutlined, PlusOutlined } from '@ant-design/icons';

import useLanguage from '@/locale/useLanguage';
import Loading from '@/components/Loading';

function SaveForm({ form, translate }) {
  const handelClick = () => {
    form.submit();
  };

  return (
    <Button onClick={handelClick} type="primary" icon={<PlusOutlined />}>
      {translate('update')}
    </Button>
  );
}

export default function UpdateItem({ config, UpdateForm }) {
  const translate = useLanguage();
  let { entity } = config;

  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { current, isLoading, isSuccess } = useSelector(selectUpdatedItem);
  const [form] = Form.useForm();

  const resetErp = {
    removed: false,
    enabled: true,
    name: '',
    group: '',
    goal: '',
    topic: '',
    promptEnhancers: [],
    prompt_output: '',
    chatgptSettings: [],
    useCache: false,
    cacheTimeoutUnit: null,
    cacheTimeoutValue: 0,
    cacheConditions: [],
    description: ''
  };

  const [currentErp, setCurrentErp] = useState(current ?? resetErp);

  const { id } = useParams();
  const [topicId, setTopicId] = useState(current ? current.topic : resetErp.topic);

  const onSubmit = (fieldsValue) => {
    setTopicId(fieldsValue.topic);
    let dataToUpdate = { ...fieldsValue };
    dispatch(erp.update({ entity, id, jsonData: dataToUpdate }));
  };
  useEffect(() => {
    if (isSuccess) {
      form.resetFields();
      dispatch(erp.resetAction({ actionType: 'update' }));
      navigate(`/${entity.toLowerCase()}/${topicId}`);
    }
  }, [isSuccess]);

  useEffect(() => {
    if (current) {
      setCurrentErp(current);
      setTopicId(current.topic._id);
      let formData = { ...current };
      form.resetFields();
      form.setFieldsValue(formData);
    }
  }, [current]);

  return (
    <>
      <PageHeader
        onBack={() => {
          window.history.back();
        }}
        title={translate('update')}
        ghost={false}
        extra={[
          <Button
            key={`${uniqueId()}`}
            onClick={() => {
              navigate(`/${entity.toLowerCase()}/${topicId}`);
            }}
            icon={<CloseCircleOutlined />}
          >
            {translate('Cancel')}
          </Button>,
          <SaveForm translate={translate} form={form} key={`${uniqueId()}`} />,
        ]}
        style={{
          padding: '20px 0px',
        }}
      ></PageHeader>
      <Divider dashed />
      <Loading isLoading={isLoading}>
        <Form form={form} layout="vertical" onFinish={onSubmit} initialValues={{ cacheTimeoutValue: '0' }}>
          <UpdateForm current={current} />
        </Form>
      </Loading>
    </>
  );
}
